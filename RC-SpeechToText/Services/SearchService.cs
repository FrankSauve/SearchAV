using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RC_SpeechToText.Services
{
    public class SearchService
    {
        public string PerformSearch(string searchTerms, List<Word> wordInfo)
        {
            var timeStampOfTerms = new List<string>();
            searchTerms = searchTerms.Trim();

            string[] arrayTerms;
			
            if (!string.IsNullOrEmpty(searchTerms))
            {
                arrayTerms = searchTerms.Split(' ');  
            }
            else
            {
                return "";
            }

            Words[] words = StringToWordList(wordInfo);

            for (var i = 0; i < words.Length; i++)
            {
                if (words[i].Word.Equals(arrayTerms[0]))
                {
                    for (var j = 0; j < arrayTerms.Length; j++)
                    {
                        if (j < words.Length)
                        {
                            if (!words[i + j].Word.Equals(arrayTerms[j]))
                            {
                                break;
                            }
                            else if (words[i + j].Word.Equals(arrayTerms[j]) && j == arrayTerms.Length - 1)
                            {
                                timeStampOfTerms.Add(TimeSpan.FromSeconds(words[i].StartTime.Seconds).ToString(@"g"));
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
            var result = string.Join(", ", timeStampOfTerms.ToArray());

            return result;
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
