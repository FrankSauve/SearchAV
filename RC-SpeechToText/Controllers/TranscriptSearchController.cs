using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RC_SpeechToText.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class TranscriptSearchController : Controller
    {
        [HttpPost("[action]")]
        public string SearchTranscript(string searchTerms, string jsonResponse)
        {
            //Gets JSON as a string and the de serialize it into an object.
            FullGoogleResponse fullResponse = JsonConvert.DeserializeObject<FullGoogleResponse>(jsonResponse);

            //Check if the search terms are in the transcript
            List<string> timeStampOfTerms = new List<string>(); // Saves all instances of words timestamps
            string[] arrayTerms = searchTerms.Split(' '); // Having an array of search terms to help when searching for timestamps  
            Words[] words = fullResponse.Words; // For clearer code instead of calling the full variable

            //First check if serch terms are in the transcript, if they are look at where the word instances are located
            if (fullResponse.Transcript.IndexOf(searchTerms, StringComparison.OrdinalIgnoreCase) >= 0) {

                //For each words check if it is what we were lonking for.
                for (int i = 0; i < words.Length; i++) {

                    //If first word of search term is equal to this current word, check if consecutive terms are equal.
                    if (words[i].Word.Equals(arrayTerms[0], StringComparison.InvariantCultureIgnoreCase)) {
                        for (int j = 0; j < arrayTerms.Length; j++) {

                            //Make sure j doesn't go out of words range
                            if (j < words.Length)
                            {
                                // If the next words in the sequence aren't the same break
                                if (!words[i + j].Word.Equals(arrayTerms[j], StringComparison.InvariantCultureIgnoreCase))
                                {
                                    break;
                                }
                                //If the last words we are looking for are equal, add this timestamp to our current list and increment i.
                                else if (words[i + j].Word.Equals(arrayTerms[j], StringComparison.InvariantCultureIgnoreCase) && j == arrayTerms.Length - 1)
                                {
                                    //Adding the timestamp in the appropriate format
                                    timeStampOfTerms.Add(TimeSpan.FromSeconds(words[i].StartTime.Seconds).ToString(@"hh\:mm\:ss\:fff"));
                                    i = i + j;

                                }
                            }
                            else {
                                break;
                            }
                                
            }
                        }
                    }
                }
            var result = String.Join(", ", timeStampOfTerms.ToArray());




            return result;

        }
      
    }

    public class FullGoogleResponse {
        public string Transcript { get; set; }
        public double Confidence { get; set; }
        public Words[] Words { get; set; }
    }

    public class Words {
        public string Word { get; set; }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }

    }

    public class Time {

        public int Seconds { get; set; }
        public int Nanos { get; set; }

    }
}