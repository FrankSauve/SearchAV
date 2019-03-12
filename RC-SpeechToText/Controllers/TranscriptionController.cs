using System;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Version = RC_SpeechToText.Models.Version;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
		private readonly TranscriptionService _transcriptionService;
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public TranscriptionController(SearchAVContext context, ILogger<TranscriptionController> logger)
        {
			_transcriptionService = new TranscriptionService(context);
            _logger = logger;
        }

        [HttpPost("[action]/{userId}/{versionId}")]
        public async Task<IActionResult> SaveTranscript(int userId, int versionId, string newTranscript)
        {
			var saveResult = await _transcriptionService.SaveTranscript(userId, versionId, newTranscript);
			if(saveResult.Error != null)
			{
				return BadRequest(saveResult.Error);
			}

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
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all versions");
                return Ok(_transcriptionService.Index());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all versions");
                return BadRequest("Get all versions failed.");
            }
        }

        /// <summary>
        /// Returns timestamps of searched terms
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        [HttpGet("[action]/{versionId}/{searchTerms}")]
        public async Task<IActionResult> SearchTranscript(string searchTerms, int versionId)
        {
            try
            {
                return Ok(await _transcriptionService.SearchTranscript(searchTerms, versionId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all words for versionId: " + versionId);
                return BadRequest("Error fetching active version with fileId: " + versionId);
            }
        }


        [HttpGet("[action]/{fileId}/{documentType}")]
        public async Task<IActionResult> DownloadTranscript(string documentType, int fileId)
        {
            _logger.LogInformation(documentType);

            var version = _context.Version.Where(v => v.FileId == fileId).Where(v => v.Active == true).SingleOrDefault(); //Gets the active version (last version of transcription)
            var rawTranscript = version.Transcription;
			var transcript = rawTranscript.Replace("<br>", "\n ");


			var exportResult = await Task.Run(async () => {
				var exportTranscriptionService = new ExportTranscriptionService();
				if (documentType == "doc")
				{
					return exportTranscriptionService.CreateWordDocument(transcript);
				}
				else if(documentType == "googleDoc")
				{
					return exportTranscriptionService.CreateGoogleDocument(transcript);
				}
				else if(documentType == "srt")
				{
					var words = await _context.Word.Where(v => v.VersionId == version.Id).ToListAsync();
					if (words.Count > 0)
						return exportTranscriptionService.CreateSRTDocument(transcript, words);
					else
						return false;
				}
				else
				{
					_logger.LogInformation("Invalid doc type");
					return false;
				}
			});

            if (exportResult)
            {
                _logger.LogInformation("Downloaded transcript: " + transcript);
                return Ok();
            }
            else
            {
                return BadRequest("Error while trying to download transcription");
            }
        }
    }
}