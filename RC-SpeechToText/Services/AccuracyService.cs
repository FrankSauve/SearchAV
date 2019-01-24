using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    public class AccuracyService
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
