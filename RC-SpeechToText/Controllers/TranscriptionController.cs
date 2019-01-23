using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Http;
using System.IO;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;

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
        public IActionResult ConvertAndTranscribe(IFormFile audioFile)
        {
            // Create the directory
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\wwwroot\assets\Audio\");
            _logger.LogInformation("Created directory /Audio");

            // Saves the file to the audio directory
            var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\assets\Audio\" + audioFile.FileName;
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
            GoogleResult result = TranscriptionService.GoogleSpeechToText(convertedFileLocation);

            // Delete the converted file
            converter.DeleteMonoFile(convertedFileLocation);
            _logger.LogInformation("Deleted " + convertedFileLocation);

            // Create file object
            //TODO: save the transcription first in a Version and in the Transcription table, then save the id into this object
            //TODO: get the type of the object, if it is a Video or an Audio file 
            Models.File file = new Models.File
            {
                Title = audioFile.FileName,
                FilePath = filePath,
                //Transcription = result.GoogleResponse.Alternatives[0].Transcript //it has to be an id that refers to the table transcription and then to the table Versions. 
            };

            // Add file object to database
            _context.File.Add(file);
            _logger.LogInformation("Added file with title: " + file.Title + " to the database");

            // Return the transcription
            return Ok(result);

        }

        
    }
}