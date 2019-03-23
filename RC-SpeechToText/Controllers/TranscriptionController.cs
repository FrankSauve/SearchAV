using System;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using RC_SpeechToText.Services;
using RC_SpeechToText.Filters;
using System.Linq;
using RC_SpeechToText.Exceptions;

namespace RC_SpeechToText.Controllers
{
    [ServiceFilter(typeof(ControllerExceptionFilter))]
    [ServiceFilter(typeof(LoggingActionFilter))]
    [Authorize]
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
		private readonly TranscriptionService _transcriptionService;

        public TranscriptionController(SearchAVContext context)
        {
			_transcriptionService = new TranscriptionService(context);
        }

        [HttpPost("[action]/{versionId}")]
        public async Task<IActionResult> SaveTranscript(Guid versionId, string newTranscript)
        {
            var emailClaim = HttpContext.User.Claims;
            var emailString = emailClaim.FirstOrDefault(c => c.Type == "email").Value;

            var saveResult = await _transcriptionService.SaveTranscript(emailString, versionId, newTranscript);
			           
            return Ok(saveResult.Version);
        }
        
        /// <summary>
        /// Returns all versions
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return Ok(await _transcriptionService.Index());
            }
            catch
            {
                return BadRequest("Get all versions failed.");
            }
        }

        [HttpGet("[action]")]
        public IActionResult Testing()
        {
            return Ok(ModifyTimeStampService.CommonWords());
        }

        /// <summary>
        /// Returns timestamps of searched terms
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        [HttpGet("[action]/{versionId}/{searchTerms}")]
        public async Task<IActionResult> SearchTranscript(Guid versionId, string searchTerms)
        {
            return Ok(await _transcriptionService.SearchTranscript(versionId, searchTerms));
        }


        [HttpGet("[action]/{fileId}/{documentType}")]
        public async Task<IActionResult> DownloadTranscript(string documentType, Guid fileId)
        {
			var result = await _transcriptionService.DownloadTranscription(documentType, fileId);

			if(result != null)
			{
                throw new ControllerExceptions("Error while trying to download transcription");
			}

			return Ok();
        }
    }
}