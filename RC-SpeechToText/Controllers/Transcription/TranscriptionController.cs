﻿using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Http;
using System.IO;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;

namespace RC_SpeechToText.Controllers
{
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;

        public TranscriptionController(SearchAVContext context, ILogger logger)
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
        public IActionResult ConvertAndTranscribe(IFormFile audioFile)
        {
            // Create the directory
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Audio");
            _logger.LogInformation("Created directory /Audio");

            // Saves the file to the audio directory
            var filePath = Directory.GetCurrentDirectory() + "\\Audio\\" + audioFile.FileName;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                audioFile.CopyTo(stream);
            }
            _logger.LogInformation("Saved audio file " + audioFile.FileName + " in /audio");

            // Once we get the file path(of the uploaded file) from the server, we use it to call the converter
            Converter converter = new Converter();
            // Call converter to convert the file to mono and bring back its file path. 
            string convertedFileLocation = converter.FileToWav(filePath);
            _logger.LogInformation("Audio file " + audioFile.FileName + " converted to wav at " + convertedFileLocation);

            // Call the method that will get the transcription
            GoogleResult result = GoogleSpeechToText(convertedFileLocation);

            // Delete the converted file
            converter.DeleteMonoFile(convertedFileLocation);
            _logger.LogInformation("Deleted " + convertedFileLocation);

            // Create video object
            Video video = new Video
            {
                Title = audioFile.FileName,
                VideoPath = filePath,
                Transcription = result.GoogleResponse.Alternatives[0].Transcript
            };

            // Add video object to database
            _context.Video.Add(video);
            _logger.LogInformation("Added video with title: " + video.Title + " to the database");

            // Return the transcription
            return Ok(result);

        }

        private  GoogleResult GoogleSpeechToText(string inputFilePath)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = "fr-ca",
                EnableWordTimeOffsets = true
            }, RecognitionAudio.FromFile(inputFilePath));

            var googleResult = new GoogleResult
            {
                GoogleResponse= response.Results[0]
            };

            return googleResult;
        }
    }
}