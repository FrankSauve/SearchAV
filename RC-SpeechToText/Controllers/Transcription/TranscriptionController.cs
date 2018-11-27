using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using RC_SpeechToText.Utils;

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
        [HttpPost("[action]")]
        public GoogleResult ConvertAndTranscribe(IFormFile audioFile)
        {
            // Create the directory
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Audio");

            // Saves the file to the audio directory
            var filePath = Directory.GetCurrentDirectory() + "\\Audio\\" + audioFile.FileName;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                audioFile.CopyTo(stream);
            }

            //once we get the file path(of the uploaded file) from the server, we use it to call the converter
            Converter converter = new Converter();
            //call converter to convert the file to mono and bring back its file path. 
            string convertedFileLocation = converter.FileToWav(filePath);

            //call the method that will get the transcription
            GoogleResult result = GoogleSpeechToText(convertedFileLocation);

            //delete the converted file
            converter.DeleteMonoFile(convertedFileLocation); 

            //return the transcription
            return result;

        }

        private  GoogleResult GoogleSpeechToText(string inputFilePath)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = "fr-ca",
                EnableWordTimeOffsets = true
            }, RecognitionAudio.FromFile(inputFilePath));

            var googleResult = new GoogleResult
            {
                GoogleResponse= response.Results[0]
            };

            return googleResult;
        }
    }
}