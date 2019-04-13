using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAPICodePack.Shell;
using RC_SpeechToText.Infrastructure;
using RC_SpeechToText.Models;
using RC_SpeechToText.Models.DTO.Incoming;
using RC_SpeechToText.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RC_SpeechToText.Services
{
	public class ConvertionService
	{

		private readonly SearchAVContext _context;
		private readonly string _bucketName = "rc-retd-stt-dev";
		private readonly AppSettings _appSettings;


		public ConvertionService(SearchAVContext context, AppSettings appSettings)
		{
			_context = context;
			_appSettings = appSettings;
		}

		public async Task<Models.Version> ConvertAndTranscribe(ConvertionDTO convertionDTO)
		{
			// Get user id by email
			var user = await _context.User.Where(u => u.Email == convertionDTO.UserEmail).FirstOrDefaultAsync();

			if (user == null)
			{
				return null;
			}

			var streamIO = new IOInfrastructure();

			var converter = new Converter();

			var filePath = GetFilePath(convertionDTO, converter, streamIO);

			CreateThumbnail(convertionDTO, streamIO, filePath, converter);

			var words = await CreateWords(streamIO, filePath, converter);

			if (words == null)
			{
				return null;
			}

			var duration = GetFileDuration(filePath);

			var fileType = converter.GetFileType(filePath);

			var transcription = CreateTranscription(words);

			var ext = System.IO.Path.GetExtension(filePath);

			var file = await CreateFile(convertionDTO, user, duration, fileType, ext);

			var version = await CreateVersion(user, transcription, words, file);

			// Send email to user that the transcription is done
			var emailService = new EmailInfrastructure();
			emailService.SendTranscriptionDoneEmail(convertionDTO.UserEmail, file);

			// Return the version
			return version;
		}

		private async Task<Models.Version> CreateVersion(User user, string transcription, List<Word> words, File file)
		{
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
			words.ForEach(async x =>
			{
				x.VersionId = version.Id;
				x.Position = i;
				await _context.Word.AddAsync(x);
				i++;
			});
			await _context.SaveChangesAsync();

			return version;
		}

		private async Task<File> CreateFile(ConvertionDTO convertionDTO, User user, string duration, string fileType, string ext)
		{
			var file = new File
			{
				Title = (convertionDTO.Title == "" ? convertionDTO.AudioFile.FileName : convertionDTO.Title + ext),
				FilePath = _appSettings.AudioPathNoRoot + (convertionDTO.Title == "" ? convertionDTO.AudioFile.FileName : convertionDTO.Title + ext),
				FileFlag = FileFlag.Automatise,
				Description = convertionDTO.Description,
				UserId = user.Id,
				DateAdded = DateTime.Now,
				Type = fileType,
				ThumbnailPath = _appSettings.ThumbnailPathNoRoot + (convertionDTO.Title == "" ? convertionDTO.AudioFile.FileName : convertionDTO.Title) + ".jpg",
				Duration = duration
			};
			await _context.File.AddAsync(file);
			await _context.SaveChangesAsync();
			return file;
		}

		private void CreateThumbnail(ConvertionDTO convertionDTO, IOInfrastructure streamIO, string filePath, Converter converter)
		{
			var thumbnailPath = streamIO.GetPathAndCreateDirectory(_appSettings.ThumbnailPath);
			var thumbnailImage = converter.CreateThumbnail(
				filePath, 
				thumbnailPath + (convertionDTO.Title == "" ? convertionDTO.AudioFile.FileName : convertionDTO.Title) + ".jpg", 1000);
		}

		private async Task<List<Word>> CreateWords(IOInfrastructure streamIO, string filePath, Converter converter)
		{
			// Call converter to convert the file to mono and bring back its file path. 
			var convertedFileLocation = converter.FileToWav(filePath);

			var words = await CreateWords(convertedFileLocation);

			// Delete the converted file
			streamIO.DeleteFile(convertedFileLocation);
			return words;
		}

		private string GetFilePath(ConvertionDTO convertionDTO, Converter converter, IOInfrastructure streamIO)
		{
			var filePath = streamIO.CopyAudioToStream(convertionDTO.AudioFile, _appSettings.AudioPath);
			if (convertionDTO.Title != "")
			{
				var newFilePath = converter.RenameFile(filePath, convertionDTO.Title);
				filePath = newFilePath;
			}

			return filePath;
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

			transcription = transcription.First().ToString().ToUpper() + transcription.Substring(1);
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
	}
}
