using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RC_SpeechToText.Models;
using Microsoft.AspNetCore.Authorization;

namespace RC_SpeechToText.Controllers.Transcription
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/SavingTranscript")]
    public class SavingTranscriptController : Controller
    {

        private readonly SearchAVContext _context;
        private readonly ILogger _logger;

        public SavingTranscriptController(SearchAVContext context, ILogger<SavingTranscriptController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // TODO: Change saving to work with new DB
        [HttpPost("[action]")]
        public IActionResult SaveTranscript(string fileId, string oldTranscript, string newTranscript)
        {
            _logger.LogInformation("fileId: " + fileId);
            _logger.LogInformation("New transcript: " + newTranscript);
            _logger.LogInformation("Old transcript: " + oldTranscript);

            //Converting fileId to an integer in oder to find the corresponding file
            int id = Int32.Parse(fileId);

            File file = _context.File.Find(id);

            //Updating file's transcription
            _logger.LogInformation("file old transcript: " + file.TranscriptionId);
            //file.TranscriptionId = newTranscript;
            _logger.LogInformation("file new transcript: " + file.TranscriptionId);

            try
            {
                _context.File.Update(file);
                _context.SaveChanges();
                _logger.LogInformation("Updated file transcription with id: " + file.FileId);
                return Ok(file);
            }
            catch
            {
                _logger.LogError("Error updating file transcription with id: " + file.FileId);
                return BadRequest("file transcription not updated.");
            }

        }
    }
}