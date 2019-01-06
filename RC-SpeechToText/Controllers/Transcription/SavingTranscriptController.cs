using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace RC_SpeechToText.Controllers.Transcription
{
    [Produces("application/json")]
    [Route("api/SavingTranscript")]
    public class SavingTranscriptController : Controller
    {

        private readonly ILogger _logger;

        public SavingTranscriptController(ILogger<SavingTranscriptController> logger)
        {
            _logger = logger;
        }

        [HttpPost("[action]")]
        public void SaveTranscript(string oldTranscript, string newTranscript)
        {
            _logger.LogInformation("SAVING TRANSCRIPT\n");
            _logger.LogInformation("New transcript: " + newTranscript);
            _logger.LogInformation("Old transcript: " + oldTranscript);

        }
    }
}