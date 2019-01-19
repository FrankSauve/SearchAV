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

        [HttpPost("[action]")]
        public IActionResult SaveTranscript(string videoId, string oldTranscript, string newTranscript)
        {
            _logger.LogInformation("videoId: " + videoId);
            _logger.LogInformation("New transcript: " + newTranscript);
            _logger.LogInformation("Old transcript: " + oldTranscript);

            //Converting videoId to an integer in oder to find the corresponding video
            int id = Int32.Parse(videoId);

            Video video = _context.Video.Find(id);

            //Updating video's transcription
            _logger.LogInformation("Video old transcript: " + video.Transcription);
            video.Transcription = newTranscript;
            _logger.LogInformation("Video new transcript: " + video.Transcription);

            try
            {
                _context.Video.Update(video);
                _context.SaveChanges();
                _logger.LogInformation("Updated video transcription with id: " + video.VideoId);
                return Ok(video);
            }
            catch
            {
                _logger.LogError("Error updating video transcription with id: " + video.VideoId);
                return BadRequest("Video transcription not updated.");
            }

        }
    }
}