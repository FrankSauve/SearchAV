using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    public class SearchService
    {

        //Performs the search on the terms
        public string PerformSearch(string searchTerms, List<Word> wordInfo)
        {

            //Check if the search terms are in the transcript
            var timeStampOfTerms = new List<string>(); // Saves all instances of words timestamps
            searchTerms = searchTerms.Trim();

            string[] arrayTerms;

            //Make sure the user did not pass an empty string
            if (!string.IsNullOrEmpty(searchTerms))
            {
                arrayTerms = searchTerms.Split(' '); // Having an array of search terms to help when searching for timestamps  
            }
            else
            {
                return "";
            }

            Words[] words = StringToWordList(wordInfo); // For clearer code instead of calling the full variable

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
                                timeStampOfTerms.Add(TimeSpan.FromSeconds(words[i].StartTime.Seconds).ToString(@"g"));
                                timeStampOfTerms.Add(TimeSpan.FromSeconds(words[i+j].StartTime.Seconds).ToString(@"g"));
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

            //Getting all timestamps and converting them to string to make it easier when passing to frontend
            if (timeStampOfTerms.Count > 0)
            {
                var temp = new List<string>();
                for (var i = 0; i < timeStampOfTerms.Count; i += 2)
                {
                    string s = (timeStampOfTerms[i] + "-" + timeStampOfTerms[i + 1]);
                    temp.Add(s);
                }
                
                var result = String.Join(", ", temp.ToArray());
                return result;
            }
            else
            {
                return "";
            }
        }

        static public List<File> SearchDescriptionAndTitle(List<File> files, string search)
        {
            search = search.Trim();

            var filesContainDescription = new List<File>();

            foreach (var file in files)
            {
                if (file.Description != null)
                {
                    if (file.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        filesContainDescription.Add(file);

                        //If file is added here we do not want to add it again if it has a title match too
                        continue;
                    }
                }
                if (file.Title != null)
                {
                    if (file.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        filesContainDescription.Add(file);
                    }
                }
            }

            return filesContainDescription;
        }


        //Converts the new database Model to the one previously used, 
        //done this way to keep same algorithm used before.
        private Words[] StringToWordList(List<Word> wordInfo)
        {
            List<Words> allWords = new List<Words>();

            foreach (Word x in wordInfo)
            {
                Regex regex = new Regex(@"([\d.]+)");
                string match = regex.Match(x.Timestamp).ToString();
                var wordToAdd = new Words
                {
                    Word = x.Term,
                    StartTime = new Time
                    {
                        Seconds = Convert.ToDouble(match)
                    },
                };
                allWords.Add(wordToAdd);
            }
            return allWords.ToArray();
        }
    }
}
