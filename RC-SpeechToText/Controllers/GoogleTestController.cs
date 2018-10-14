using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Speech.V1;

namespace RC_SpeechToText.Controllers
{
    [Route("api/[controller]")]
    public class GoogleTestController : Controller
    {
        // GET: /api/googletest/speechtotext
        [HttpGet("[action]")]
        public string SpeechToText()
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = "fr",
                EnableWordTimeOffsets = true
            }, RecognitionAudio.FromFile("C:\\Users\\franc\\Downloads\\f_m.wav")); // Add file name here

            return response.Results.ToString();
        }
    }
}