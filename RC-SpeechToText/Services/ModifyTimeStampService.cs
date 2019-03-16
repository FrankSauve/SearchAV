using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RC_SpeechToText.Services
{
    public class ModifyTimeStampService
    {
        public List<Word> ModifyTimestamps(List<Word> oldWords, string newTranscript, Guid newVersionId)
        {
			var newTranscriptNoBr = newTranscript
				.Replace("<br>", " ")
				.Split(" ")
				.Select(str => str.Trim())
				.Where(x => !string.IsNullOrEmpty(x))
				.ToList();
			
            List<Word> newWords = new List<Word>();

            if (newTranscriptNoBr.Count == oldWords.Count)
            {
                newWords = HandleEdited(oldWords, newTranscript, newVersionId, newTranscriptNoBr);
            }
            else if (newTranscriptNoBr.Count < oldWords.Count)
            {
                newWords = HandleDeleted(oldWords, newTranscript, newVersionId, newTranscriptNoBr);
            }
            else
            {
                newWords = HandleAdded(oldWords, newTranscript, newVersionId, newTranscriptNoBr);
            }
            return newWords;
        }

        private List<Word> HandleEdited(List<Word> oldWords, string newTranscript, Guid newVersionId, List<string> newTranscriptList)
        {
            List<Word> newWords = new List<Word>();
            for (int i = 0; i < newTranscriptList.Count; i++)
            {
                newWords.Add(new Word { Term = newTranscriptList[i], Timestamp = oldWords[i].Timestamp, VersionId = newVersionId });
            }
            return newWords;
        }

        private List<Word> HandleDeleted(List<Word> oldWords, string newTranscript, Guid newVersionId, List<string> newTranscriptList)
        {
            List<Word> newWords = new List<Word>();

            var iterateOld = 0;
            for (int i = 0; i < newTranscriptList.Count; i++)
            {
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

        private List<Word> HandleAdded(List<Word> oldWords, string newTranscript, Guid newVersionId, List<string> newTranscriptList)
        {
            List<Word> newWords = new List<Word>();
            var iterateOld = 0;
			
            for (int i = 0; i < newTranscriptList.Count; i++)
            {
                var currentNewWord = newTranscriptList[i];
                if (currentNewWord.Equals(oldWords[iterateOld].Term, StringComparison.InvariantCultureIgnoreCase))
                {
                    newWords.Add(new Word { Term = currentNewWord, Timestamp = oldWords[iterateOld].Timestamp, VersionId = newVersionId });
                    if (iterateOld != oldWords.Count - 1)
                    {
                        iterateOld++;
                    }
                }
                else
                {
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
