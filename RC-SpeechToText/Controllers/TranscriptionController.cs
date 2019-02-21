﻿using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");
        private readonly string _bucketName = "rc-retd-stt-dev";

        public TranscriptionController(SearchAVContext context, ILogger<TranscriptionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Generates an automatic transcript using google cloud.
        /// GET: /api/googletest/speechtotext
        /// </summary>
        /// <returns>GoogleResult</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> ConvertAndTranscribe(IFormFile audioFile, string userEmail)
        {
            // Create the directory
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\wwwroot\assets\Audio\");
            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Created directory /Audio");

            // Saves the file to the audio directory
            var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\assets\Audio\" + audioFile.FileName;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                audioFile.CopyTo(stream);
            }
            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Saved audio file " + audioFile.FileName + " in /audio");

            // Once we get the file path(of the uploaded file) from the server, we use it to call the converter
            Converter converter = new Converter();
            // Call converter to convert the file to mono and bring back its file path. 
            var convertedFileLocation = converter.FileToWav(filePath);

			if (convertedFileLocation == null)
			{
				return BadRequest("Une erreur ces produite lors de la convertion du fichier à Wav");
			}

			_logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Audio file " + audioFile.FileName + " converted to wav at " + convertedFileLocation);

            // Upload the mono wav file to Google Storage
            var storageObject = await TranscriptionService.UploadFile(_bucketName, convertedFileLocation);
            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Uploaded file to Google Storage bucket.");

            // Call the method that will get the transcription
            var result = TranscriptionService.GoogleSpeechToText(convertedFileLocation);

			if (result.Error != null)
			{
				return BadRequest("Une erreur ces produite lors de la transcription du fichier: " + result.Error);
			}

			// Delete the converted file
			converter.DeleteFile(convertedFileLocation);
            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Deleted " + convertedFileLocation);

            // Create thumbnail
            var thumbnailPath = Directory.GetCurrentDirectory() + @"\wwwroot\assets\Thumbnails\";
            Directory.CreateDirectory(thumbnailPath);
			var thumbnailImage = converter.CreateThumbnail(filePath, thumbnailPath + audioFile.FileName + ".jpg");

			if (thumbnailImage == null)
			{
				return BadRequest("Une erreur ces produite lors de la creation du thumbnail");
			}

			// Get user id by email
			var user = await _context.User.Where(u => u.Email == userEmail).FirstOrDefaultAsync();

			if (user == null)
			{
				return BadRequest("Une erreur ces produite lors de la récupération du user");
			}

			// Create file
			//TODO: get the type of the object, if it is a Video or an Audio file 
			var file = new Models.File
            {
                Title = audioFile.FileName,
                FilePath = filePath,
                Flag = "Automatisé",
                UserId = user.Id,
                DateAdded = DateTime.Now,
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

            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Added file with title: " + file.Title + " to the database");
            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Added version with ID: " + version.Id + " to the database");
            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Added words related to title/version: " + file.Title + "/" + version.Id + " to the database");
            // Return the transcription
            return Ok(version);

        }

        
    }
}