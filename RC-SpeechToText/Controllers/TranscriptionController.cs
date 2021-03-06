﻿using System;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Services;
using RC_SpeechToText.Filters;
using System.Linq;
using RC_SpeechToText.Exceptions;
using Microsoft.Extensions.Options;
using RC_SpeechToText.Models.DTO.Outgoing;

namespace RC_SpeechToText.Controllers
{
    [ServiceFilter(typeof(ControllerExceptionFilter))]
    [ServiceFilter(typeof(LoggingActionFilter))]
    [Authorize]
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
		private readonly TranscriptionService _transcriptionService;
        private readonly FileService _fileService;
        private readonly ExportTranscriptionService _exportTranscriptionService;
		private AppSettings _appSettings { get; set; }

		public TranscriptionController(SearchAVContext context, IOptions<AppSettings> settings)
        {
			_appSettings = settings.Value;
			_transcriptionService = new TranscriptionService(context, _appSettings);
            _fileService = new FileService(context, _appSettings);
            _exportTranscriptionService = new ExportTranscriptionService(context, _appSettings);
        }

        [HttpPost("{versionId}")]
        public async Task<IActionResult> SaveTranscript(Guid versionId, string newTranscript)
        {
            var emailClaim = HttpContext.User.Claims;
            var emailString = emailClaim.FirstOrDefault(c => c.Type == "email").Value;

            var saveResult = await _transcriptionService.SaveTranscript(emailString, versionId, newTranscript);

            return Ok(saveResult.Version);
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
            var outDTO = new OutSearchTranscriptDTO { VersionId = versionId, SearchTerms = searchTerms };
            return Ok(await _transcriptionService.SearchTranscript(outDTO));
        }

        /// <summary>
        /// Downloads a file (mp4, srt, burned mp4)
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="fileId"></param>
        /// <returns>File</returns>
        [HttpGet("[action]/{fileId}/{documentType}")]
        public async Task<IActionResult> DownloadTranscript(string documentType, Guid fileId)
        {
            var outDTO = new OutDownloadTranscriptDTO { FileId = fileId, DocumentType = documentType };
            var result = await _transcriptionService.PrepareDownload(outDTO);

            if (result != null)
            {
                throw new ControllerExceptions("Error while trying to download transcription");
            }

            if(documentType == "srt" || documentType == "video" || documentType == "videoburn")
            {
                // Return the file to download
                var file = await _fileService.GetFileById(fileId);
                byte[] fileBytes = _exportTranscriptionService.GetFileBytes(documentType, file);
                var contentType = "APPLICATION/octet-stream";
                return File(fileBytes, contentType, file.Title);
            }
            else
            {
                return Ok();
            }
        }
    }
}