using System;
using Microsoft.AspNetCore.Http;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using RC_SpeechToText.Infrastructure;
using Microsoft.WindowsAPICodePack.Shell;
using RC_SpeechToText.Exceptions;

namespace RC_SpeechToText.Services
{
	public class ConvertionService
	{

		private readonly SearchAVContext _context;
		private readonly string _bucketName = "rc-retd-stt-dev";

        public ConvertionService(SearchAVContext context)
		{
			_context = context;
		}

		public async Task<Models.Version> ConvertAndTranscribe(IFormFile audioFile, string userEmail, string descriptionFile, string title)
		{
			
            // Get user id by email
			var user = await _context.User.Where(u => u.Email == userEmail).FirstOrDefaultAsync();

			if (user == null)
			{
				return null;
			}

			var streamIO = new IOInfrastructure();

			var filePath = streamIO.CopyAudioToStream(audioFile, @"\wwwroot\assets\Audio\");
            string ext = System.IO.Path.GetExtension(filePath);


            // Once we get the file path(of the uploaded file) from the server, we use it to call the converter
            Converter converter = new Converter();

            if (title != "")
            {
                var newFilePath = converter.RenamFile(filePath, title);
                filePath = newFilePath;
            }

			// Create thumbnail
			var thumbnailPath = streamIO.GetPathAndCreateDirectory(@"\wwwroot\assets\Thumbnails\");
			var thumbnailImage = converter.CreateThumbnail(filePath, thumbnailPath + (title == "" ? audioFile.FileName : title) + ".jpg", 1000);

			if (thumbnailImage == null)
			{
				return null;
			}

			// Call converter to convert the file to mono and bring back its file path. 
			var convertedFileLocation = converter.FileToWav(filePath);

			var words = await CreateWords(convertedFileLocation);

            //Get the duration of the file
            var duration = GetFileDuration(filePath); 

            // Delete the converted file
            streamIO.DeleteFile(convertedFileLocation);

			if (words == null)
			{
				return null;
			}

			//Gets type of file (audio or video)
			var fileType = converter.GetFileType(filePath);

			//Create transcription out of the words and capitalizing the first letter
			var transcription = CreateTranscription(words);
            transcription = transcription.First().ToString().ToUpper() + transcription.Substring(1);


            // Create file
            var file = new File
			{
                Title = (title == "" ? audioFile.FileName : title + ext),
				FilePath = @"\assets\Audio\" + (title == "" ? audioFile.FileName : title + ext),
				FileFlag = FileFlag.Automatise,
                Description = descriptionFile, 
                UserId = user.Id,
				DateAdded = DateTime.Now,
				Type = fileType,
				ThumbnailPath = @"\assets\Thumbnails\" + (title == "" ? audioFile.FileName : title) + ".jpg",
                Duration = duration
            };
			await _context.File.AddAsync(file);
			await _context.SaveChangesAsync();

			// Create version
			var version = new Models.Version
			{
				UserId = user.Id,
				FileId = file.Id,
                HistoryTitle = "CRÉATION DU FICHIER",
                Transcription = transcription,
				DateModified = DateTime.Now,
				Active = true
			};
			await _context.Version.AddAsync(version);
			await _context.SaveChangesAsync();

            //Adding all words and their timestamps to the Word table
            int i = 0;
			words.ForEach(async x => {
				x.VersionId = version.Id;
                x.Position = i;
				await _context.Word.AddAsync(x);
                i++;
			});
			await _context.SaveChangesAsync();

			// Send email to user that the transcription is done
			var emailService = new EmailInfrastructure();
			emailService.SendTranscriptionDoneEmail(userEmail, file);

			// Return the transcription
			return version;
		}

        //this method gets the duration of the file and formats it to hh:mm:ss. 
        private string GetFileDuration(string fileName)
        {
            ShellFile so = ShellFile.FromFilePath(fileName);
            double.TryParse(so.Properties.System.Media.Duration.Value.ToString(), out double nanoseconds);
            var milliseconds = (nanoseconds * 0.0001);
            TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds);
            var duration = ts.ToString(@"hh\:mm\:ss");

            return duration; 
        }

        private string CreateTranscription(List<Word> words)
		{
			string transcription = "";
			foreach (var word in words)
			{
				transcription += word.Term + " ";
			}

			return transcription;
		}

		private async Task<List<Word>> CreateWords(string convertedFileLocation)
		{
			if (convertedFileLocation == null)
			{
				return null;
			}

			// Upload the mono wav file to Google Storage
			var storageObject = await GoogleRepository.UploadFile(_bucketName, convertedFileLocation);

			// Call the method that will get the transcription
			var googleResult = GoogleRepository.GoogleSpeechToText(_bucketName, storageObject.Name);

			//Persistent to domain model
			var words = googleResult.GoogleResponse.Results
				.SelectMany(x => x.Alternatives[0].Words)
				.Select(word => new Word { Term = word.Word, Timestamp = word.StartTime.ToString() })
				.ToList();

			await GoogleRepository.DeleteObject(_bucketName, storageObject.Name);

			return words;
		}

        public async Task<int> ProcessFiles(List<IFormFile> files, string userEmail, string description, string title)
        {
            var converter = new Converter();
            var count = 0;
            foreach (var file in files)
            {
                if (converter.GetFileType(file.FileName) == "N/A")
                {
                    count++;
                    continue;
                }
                else
                {
                    var version = await ConvertAndTranscribe(file, userEmail, description, file.FileName);
                    if (version == null)
                    {
                        count++;
                        continue;
                    }
                }
            }
            return count;

        }

    }
}
