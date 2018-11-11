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
            List<int> instancesOfTerms; //Saves all instances to get word index and ge timestamp
            int indexPosition = 0; //Make sure we dont go out of subscript range when searching through transcription

            //First check if serch terms are in the transcript, if they are look at where the word instances are located
            if (fullResponse.Transcript.IndexOf(searchTerms, StringComparison.OrdinalIgnoreCase) >= 0) {

                return "Exists in transcript";


            }
            else {
                return "Does not exist in transcript";
            }


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