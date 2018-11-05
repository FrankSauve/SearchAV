using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace RC_SpeechToText.Controllers
{
    [Route("api/[controller]")]
    public class SampleTestController : Controller
    {
        /// <summary>
        /// Generates an automatic transcript using google cloud.
        /// GET: /api/googletest/speechtotext
        /// </summary>
        /// <returns>GoogleResult</returns>
        [HttpPost("[action]")]
        public GoogleResult GoogleSpeechToText(IFormFile audioFile, IFormFile srtFile)
        {
            
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                LanguageCode = "fr-ca",
                EnableWordTimeOffsets = true
            }, RecognitionAudio.FromStream(audioFile.OpenReadStream())); // Add file name here

            var manualTranscript = Helpers.CreateManualTranscipt(srtFile);
            var googleResult = new GoogleResult
            {
                GoogleResponse = response.Results[0],
                ManualTranscript = manualTranscript,
                Accuracy = Helpers.CalculateAccuracy(manualTranscript, response.Results[0].Alternatives[0].Transcript)
            };

            return googleResult;
        }
        /// <summary>
        /// Converts an mp4 video into a flac audio file
        /// GET: /api/sampletest/videotoaudio
        /// </summary>
        /// <returns>string</returns>
        [HttpGet("[action]")]
        public string VideoToAudio()
        {
            var inputLocation = "C:\\Users\\William\\Downloads\\f_d.mp4";
            var outputLocation = "C:\\Users\\William\\Downloads\\f_d.flac";
            
            var inputFile = new MediaFile { Filename = inputLocation};
            var outputFile = new MediaFile { Filename = outputLocation };


            try
            {
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    engine.Convert(inputFile, outputFile);
                }
            }
            catch (Exception ex)
            {
                return "Conversion Unsuccessful";
            }

            return "Conversion successful";
        }
    }

    public class GoogleResult
    {
        public SpeechRecognitionResult GoogleResponse { get; set; }
        public string ManualTranscript { get; set; }
        public double Accuracy { get; set; }
    }

    public class Helpers
    {
        /// <summary>
        /// Gets the manual transcript of an srt file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>string text</returns>
        public static string CreateManualTranscipt(IFormFile srtFile)
        {
            var lines = new List<string>();
            using (var reader = new StreamReader(srtFile.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    lines.Add(reader.ReadLine());
            }

            Regex rx = new Regex(@"^[0-9]*$");
            string text = "";
            foreach (string line in lines)
            {
                if (line != "")
                {
                    if (!rx.IsMatch(line[0].ToString()))
                    {
                        text += line + " ";
                    }
                }
            }
            return text;
        }

        /// <summary>
        /// Calculates a rough estimate of the accuracy by checking if words of 
        /// manual transcript are in the automated transcript.
        /// </summary>
        /// <param name="manualTranscript"></param>
        /// <param name="automatedTranscript"></param>
        /// <returns>double accuracy</returns>
        public static double CalculateAccuracy(string manualTranscript, string automatedTranscript)
        {
            string[] manualWords = manualTranscript.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries);
            string[] automatedWords = automatedTranscript.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries);

            int numWords = 0;
            foreach (string word in automatedWords)
            {
                if (manualWords.Contains(word.ToLower()))
                {
                    numWords += 1;
                }
            }

            double accuraccy = (double)numWords / (double)manualWords.Length;

            return accuraccy;
        }
    }
}