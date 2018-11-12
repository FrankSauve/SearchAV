using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;


namespace RC_SpeechToText.Controllers
{
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
        /// <summary>
        /// Generates an automatic transcript using google cloud.
        /// GET: /api/googletest/speechtotext
        /// </summary>
        /// <returns>GoogleResult</returns>
        //[HttpPost("[action]")]
        //public GoogleTranscription ManageTranscription(IFormFile audioFile)
        //{
        //once we get the file path (of the uploaded file) from the server, we use it to call the converter
        // string URL = null;

        // Utils.Converter converter;
        //call converter manager to select if the file is a video or audio 
        //  var audioFileConverted = converter.ConverterManager(URL); 
        // Controllers.GoogleTranscription googleTranscription;


        //call the method that will get the transcription 
        // var googleResponse = googleTranscription.GoogleSpeechToText2(audioFileConverted);

        //delete the converted file

        //return the transcription
        //  return googleResponse; 

        // }


        [HttpPost("[action]")]
        public  GoogleTranscriptionResult GoogleSpeechToText2(IFormFile inputFile)
        {
            // Saves the file to the audio directory
            var filePath = Directory.GetCurrentDirectory() + "\\Audio\\" + inputFile.FileName;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                inputFile.CopyTo(stream);
            }

            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = "fr-ca",
                EnableWordTimeOffsets = true
            }, RecognitionAudio.FromStream(audioFile.OpenReadStream())); // Add file name here

            var googleTranscriptionResult = new GoogleTranscriptionResult
            {
                Transcription= response.Results[0]
            };

            return googleTranscriptionResult;
        }
    }

    public class GoogleTranscriptionResult
    {
        public SpeechRecognitionResult Transcription { get; set; }
    }
}