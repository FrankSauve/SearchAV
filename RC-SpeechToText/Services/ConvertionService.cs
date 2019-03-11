using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using Google.Cloud.Speech.V1;
using RC_SpeechToText.Infrastructure;

namespace RC_SpeechToText.Services
{
    public class ConvertionService
	{

		private readonly SearchAVContext _context;
		private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");
		private readonly string _bucketName = "rc-retd-stt-dev";

		public ConvertionService(SearchAVContext context)
		{
			_context = context;
		}

		public async Task<Models.Version> ConvertAndTranscribe(IFormFile audioFile, string userEmail)
		{
			// Create the directory
			Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\wwwroot\assets\Audio\");

			// Saves the file to the audio directory
			var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\assets\Audio\" + audioFile.FileName;
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				audioFile.CopyTo(stream);
			}

			// Once we get the file path(of the uploaded file) from the server, we use it to call the converter
			Converter converter = new Converter();
			// Call converter to convert the file to mono and bring back its file path. 
			var convertedFileLocation = converter.FileToWav(filePath);
			//Gets type of file (audio or video)
			var fileType = converter.GetFileType(filePath);

			if (convertedFileLocation == null)
			{
				return null;
			}

			// Upload the mono wav file to Google Storage
			var storageObject = await GoogleRepository.UploadFile(_bucketName, convertedFileLocation);

			// Call the method that will get the transcription
			var googleResult = GoogleRepository.GoogleSpeechToText(_bucketName, storageObject.Name);

			var googleResponse = googleResult.GoogleResponse;

			//Persistent to domain model
			var words = CreateWords(googleResponse);

			//Create transcription out of the words
			var transcription = CreateTranscription(words);

			await GoogleRepository.DeleteObject(_bucketName, storageObject.Name);

			// Delete the converted file
			converter.DeleteFile(convertedFileLocation);

			// Create thumbnail
			var thumbnailPath = Directory.GetCurrentDirectory() + @"\wwwroot\assets\Thumbnails\";
			Directory.CreateDirectory(thumbnailPath);
			var thumbnailImage = converter.CreateThumbnail(filePath, thumbnailPath + audioFile.FileName + ".jpg");

			if (thumbnailImage == null)
			{
				return null;
			}

			// Get user id by email
			var user = await _context.User.Where(u => u.Email == userEmail).FirstOrDefaultAsync();

			if (user == null)
			{
				return null;
			}

			// Create file
			var file = new Models.File
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
			foreach (var word in words)
			{
				word.VersionId = version.Id;
				await _context.Word.AddAsync(word);
				await _context.SaveChangesAsync();
			}

			// Send email to user that the transcription is done
			var emailService = new EmailService();
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

		private List<Word> CreateWords(LongRunningRecognizeResponse googleResponse)
		{
			var words = new List<Word>();
			foreach (var result in googleResponse.Results)
			{
				foreach (var word in result.Alternatives[0].Words)
				{
					words.Add(new Word { Term = word.Word, Timestamp = word.StartTime.ToString() });
				}
			}

			return words;
		}
	}
}
