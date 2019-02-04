using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using RC_SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RC_SpeechToText.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TranscriptSearchController : Controller
    {
        private readonly ILogger _logger;
        private readonly SearchAVContext _context;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public TranscriptSearchController(SearchAVContext context, ILogger<TranscriptSearchController> logger)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Returns all versions
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all versions");
                return Ok(await _context.Version.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all versions");
                return BadRequest("Get all versions failed.");
            }
        }

        /// <summary>
        /// Returns active version corresponding to the fileId
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        [HttpGet("[action]/{versionId}")]
        public async Task<IActionResult> SearchTranscript(string searchTerms, int versionId)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all words for versionId: " + versionId);

                var words = await _context.Word.Where(w => w.VersionId == versionId).OrderBy(w => w.Id).ToListAsync();
                
                return Ok(words);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all words for versionId: " + versionId);
                return BadRequest("Error fetching active version with fileId: " + versionId);
            }
           
        }

        private string[] PerformSearch(string searchTerms, List<Models.Word> wordInfo) {
            /*
           _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Searching for " + searchTerms);
           //Gets JSON as a string and then deserialize it into an object.
           var fullResponse = JsonConvert.DeserializeObject<FullGoogleResponse>(jsonResponse);

           //Check if the search terms are in the transcript
           var timeStampOfTerms = new List<string>(); // Saves all instances of words timestamps
           searchTerms = searchTerms.Trim();

           string[] arrayTerms;

           //Make sure the user did not pass an empty string
           if (!String.IsNullOrEmpty(searchTerms))
           {
               arrayTerms = searchTerms.Split(' '); // Having an array of search terms to help when searching for timestamps  
           }
           else
           {
               return Ok("");
           }

           Words[] words = fullResponse.Words; // For clearer code instead of calling the full variable
           _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Searching on words: " + fullResponse.Words);

           //First check if serch terms are in the transcript, if they are look at where the word instances are located
           if (fullResponse.Transcript.IndexOf(searchTerms, StringComparison.OrdinalIgnoreCase) >= 0)
           {
               //For each words check if it is what we were looking for.
               for (var i = 0; i < words.Length; i++)
               {
                   //If first word of search term is equal to this current word, check if consecutive terms are equal.
                   if (words[i].Word.Equals(arrayTerms[0], StringComparison.InvariantCultureIgnoreCase))
                   {
                       for (var j = 0; j < arrayTerms.Length; j++)
                       {

                           //Make sure j doesn't go out of words range
                           if (j < words.Length)
                           {
                               // If the next words in the sequence aren't the same: break
                               if (!words[i + j].Word.Equals(arrayTerms[j], StringComparison.InvariantCultureIgnoreCase))
                               {
                                   break;
                               }
                               //If the last words of the search terms we are looking for are equal, add this timestamp to our current list and increment i by j.
                               else if (words[i + j].Word.Equals(arrayTerms[j], StringComparison.InvariantCultureIgnoreCase) && j == arrayTerms.Length - 1)
                               {
                                   //Adding the timestamp in the appropriate format
                                   timeStampOfTerms.Add(TimeSpan.FromSeconds(words[i].StartTime.Seconds).ToString(@"hh\:mm\:ss"));
                                   i = i + j;
                               }
                           }
                           else
                           {
                               break;
                           }
                       }
                   }
               }
           }

           //Getting all timestamps and converting them to string to make it easier when passing to frontend
           var result = String.Join(", ", timeStampOfTerms.ToArray());
           _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Time stamps of terms: " + timeStampOfTerms);
           */
            string[] result = { "x"};
           return result;
        }

        /*private Models.Words[] stringToWordList(List<String> wordInfo)
        {
            Models.Words[] allWords;

            return x;
        }*/

    }
}