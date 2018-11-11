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
            string URL = null;

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
    }
}