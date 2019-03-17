using System;
using Microsoft.AspNetCore.Http;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using RC_SpeechToText.Infrastructure;

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

		public async Task<Models.Version> ConvertAndTranscribe(IFormFile audioFile, string userEmail)
		{
			// Get user id by email
			var user = await _context.User.Where(u => u.Email == userEmail).FirstOrDefaultAsync();

			if (user == null)
			{
				return null;
			}

			var streamIO = new IOInfrastructure();

			var filePath = streamIO.CopyAudioToStream(audioFile, @"\wwwroot\assets\Audio\");

			// Once we get the file path(of the uploaded file) from the server, we use it to call the converter
			Converter converter = new Converter();

			// Create thumbnail
			var thumbnailPath = streamIO.GetPathAndCreateDirectory(@"\wwwroot\assets\Thumbnails\");
			var thumbnailImage = converter.CreateThumbnail(filePath, thumbnailPath + audioFile.FileName + ".jpg");

			if (thumbnailImage == null)
			{
				return null;
			}

			// Call converter to convert the file to mono and bring back its file path. 
			var convertedFileLocation = converter.FileToWav(filePath);

			var words = await CreateWords(convertedFileLocation);

			// Delete the converted file
			streamIO.DeleteFile(convertedFileLocation);

			if (words == null)
			{
				return null;
			}

			//Gets type of file (audio or video)
			var fileType = converter.GetFileType(filePath);

			//Create transcription out of the words
			var transcription = CreateTranscription(words);

			// Create file
			var file = new File
			{
				Title = audioFile.FileName,
				FilePath = filePath,
				Flag = "Automatisé",
				UserId = user.Id,
				DateAdded = DateTime.Now,
				Type = fileType,
				ThumbnailPath = @"\assets\Thumbnails\" + audioFile.FileName + ".jpg",
				//Description = "" 
			};
			await _context.File.AddAsync(file);
			await _context.SaveChangesAsync();

			// Create version
			var version = new Models.Version
			{
				UserId = user.Id,
				FileId = file.Id,
				Transcription = transcription,
				DateModified = DateTime.Now,
				Active = true
			};
			await _context.Version.AddAsync(version);
			await _context.SaveChangesAsync();

			//Adding all words and their timestamps to the Word table
			words.ForEach(async x => {
				x.VersionId = version.Id;
				await _context.Word.AddAsync(x);
			});
			await _context.SaveChangesAsync();

			// Send email to user that the transcription is done
			var emailService = new EmailInfrastructure();
			emailService.SendTranscriptionDoneEmail(userEmail, file);

			// Return the transcription
			return version;
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
	}
}
