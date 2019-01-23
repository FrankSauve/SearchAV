﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccuracyController : Controller
    {
        private readonly ILogger _logger;

        public AccuracyController(ILogger<AccuracyController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Generates an automatic transcript using google cloud.
        /// GET: /api/googletest/speechtotext
        /// </summary>
        /// <returns>GoogleResult</returns>
        [HttpPost("[action]")]
        public IActionResult GoogleSpeechToTextWithSrt(IFormFile audioFile, IFormFile srtFile)
        {
            _logger.LogInformation("Executing speech to text on file " + audioFile.FileName);
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                LanguageCode = "fr-ca",
                EnableWordTimeOffsets = true
            }, RecognitionAudio.FromStream(audioFile.OpenReadStream()));
            _logger.LogInformation("Speech to text done on file " + audioFile.FileName);

            var manualTranscript = AccuracyService.CreateManualTranscipt(srtFile);
            var googleResult = new GoogleResult
            {
                GoogleResponse = response.Results[0],
                ManualTranscript = manualTranscript,
                Accuracy = AccuracyService.CalculateAccuracy(manualTranscript, response.Results[0].Alternatives[0].Transcript)
            };
            _logger.LogInformation("Accuracy of audio file " + audioFile.FileName + " with srt file " + srtFile.FileName + ": " + googleResult.Accuracy);

            return Ok(googleResult);
        }
    }
}