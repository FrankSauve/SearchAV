using System;
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

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

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
            string convertedFileLocation = converter.FileToWav(filePath);
            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Audio file " + audioFile.FileName + " converted to wav at " + convertedFileLocation);

            // Call the method that will get the transcription
            GoogleResult result = TranscriptionService.GoogleSpeechToText(convertedFileLocation);

            // Delete the converted file
            converter.DeleteFile(convertedFileLocation);
            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Deleted " + convertedFileLocation);

            // Create transcription
            var transcription = new Transcription
            {
                Flag = "In Progress"
            };
            await _context.Transcription.AddAsync(transcription);
            await _context.SaveChangesAsync();

            // Get user id by email
            var user = await _context.User.Where(u => u.Email == userEmail).FirstOrDefaultAsync();

            // Create version
            var version = new Models.Version
            {
                UserId = user.UserId,
                TranscriptionId = transcription.TranscriptionId,
                Transcription = result.GoogleResponse.Alternatives[0].Transcript,
            };
            await _context.Version.AddAsync(version);
            await _context.SaveChangesAsync();

            // Create file
            //TODO: get the type of the object, if it is a Video or an Audio file 
            var file = new Models.File
            {
                Title = audioFile.FileName,
                FilePath = filePath,
                TranscriptionId = transcription.TranscriptionId,
                UserId = user.UserId,
                DateAdded = DateTime.Now
        };
            await _context.File.AddAsync(file);
            await _context.SaveChangesAsync();

            _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Added file with title: " + file.Title + " to the database");

            // Return the transcription
            return Ok(result);

        }

        
    }
}