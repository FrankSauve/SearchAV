using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RC_SpeechToText.Services
{
    public class ModifyTimeStampService
    {
        public List<Word> ModifyTimestamps(List<Word> oldWords, string newTranscript, int newVersionId)
        {
            //Removing all the line skips to have 
            var newTranscriptNoBr = newTranscript.Replace("<br>", " ");
            //Changes the new transcript to a list of words to match it against the old words                  
            var newTranscriptList = newTranscriptNoBr.Split(" ").ToList().Select(str => str.Trim()).ToList();

            //Removing Empty strings
            newTranscriptList.RemoveAll(str => string.IsNullOrEmpty(str));

            //Have to explicitely create variable to hold the Word objects
            List<Word> newWords = new List<Word>();

            //Here we have 3 cases 1) If the user only edited words 2) If the user only removed words 3) If the user kept same words as previous transcript but added new ones

            //If both array are the same size we can assume the user only edited words and can associate the right timestam to the right word
            if (newTranscriptList.Count == oldWords.Count)
            {
                newWords = HandleEdited(oldWords, newTranscript, newVersionId, newTranscriptList);
            }
            //If list of old words is larger than the new one we can assume that the user deleted some words
            else if (newTranscriptList.Count < oldWords.Count)
            {
                newWords = HandleDeleted(oldWords, newTranscript, newVersionId, newTranscriptList);
            }
            //If list of new words is larger than the old one we can assume that the user added new words
            else
            {
                newWords = HandleAdded(oldWords, newTranscript, newVersionId, newTranscriptList);
            }


            return newWords;
        }

        private List<Word> HandleEdited(List<Word> oldWords, string newTranscript, int newVersionId, List<string> newTranscriptList)
        {
            //Have to explicitely create variable to hold the Word objects
            List<Word> newWords = new List<Word>();

            for (int i = 0; i < newTranscriptList.Count; i++)
            {
                newWords.Add(new Word { Term = newTranscriptList[i], Timestamp = oldWords[i].Timestamp, VersionId = newVersionId });
            }

            return newWords;
        }

        private List<Word> HandleDeleted(List<Word> oldWords, string newTranscript, int newVersionId, List<string> newTranscriptList)
        {
            //Have to explicitely create variable to hold the Word objects
            List<Word> newWords = new List<Word>();

            var iterateOld = 0;
            for (int i = 0; i < newTranscriptList.Count; i++)
            {
                //When going through the new transcript look through old transcript to find the words that match.
                for (int j = iterateOld; j < oldWords.Count; j++)
                {
                    if (newTranscriptList[i].Equals(oldWords[j].Term, StringComparison.InvariantCultureIgnoreCase))
                    {
                        newWords.Add(new Word { Term = newTranscriptList[i], Timestamp = oldWords[j].Timestamp, VersionId = newVersionId });
                        iterateOld = j + 1;
                        break;
                    }
                }

            }

            return newWords;
        }

        private List<Word> HandleAdded(List<Word> oldWords, string newTranscript, int newVersionId, List<string> newTranscriptList)
        {
            //Have to explicitely create variable to hold the Word objects
            List<Word> newWords = new List<Word>();


            //Kept to keep track of where we are in the old list of words
            var iterateOld = 0;

            //This logic here goes through both old and new transcripts simultaneously and assign new words the transcript of the previous word
            for (int i = 0; i < newTranscriptList.Count; i++)
            {
                var currentNewWord = newTranscriptList[i];
                if (currentNewWord.Equals(oldWords[iterateOld].Term, StringComparison.InvariantCultureIgnoreCase))
                {
                    newWords.Add(new Word { Term = currentNewWord, Timestamp = oldWords[iterateOld].Timestamp, VersionId = newVersionId });
                    //Making sure we will not go over the list size
                    if (iterateOld != oldWords.Count - 1)
                    {
                        iterateOld++;
                    }
                }
                else
                {
                    //Depending on where we add the word this makes sure the word is given the timestamp of the previous word
                    if (iterateOld == 0 || iterateOld == oldWords.Count - 1)
                    {
                        newWords.Add(new Word { Term = currentNewWord, Timestamp = oldWords[iterateOld].Timestamp, VersionId = newVersionId });
                    }
                    else
                    {
                        newWords.Add(new Word { Term = currentNewWord, Timestamp = oldWords[iterateOld - 1].Timestamp, VersionId = newVersionId });
                    }
                }
            }

            return newWords;
        }
    }

    
}
