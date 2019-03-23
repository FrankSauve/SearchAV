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

        static public List<string> CommonWords()
        {

            var string1 = "su au bout du processus je suis un etre human vegan je suis un peu comme un superheros qui mange de la malbouffe";
            var string2 = "su au bout du processus je edited un added added added suis un peu comme un superheros qui mange";

            var str1List = string1
                .Replace("<br>", " ")
                .Replace(".", " ")
                .Replace(",", " ")
                .Split(" ")
                .Select(str => str.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var str2List = string2
               .Replace("<br>", " ")
               .Replace(".", " ")
               .Replace(",", " ")
               .Split(" ")
               .Select(str => str.Trim())
               .Where(x => !string.IsNullOrEmpty(x))
               .ToList();


            var subSeqTable = new int[str1List.Count + 1, str2List.Count + 1];

            //Filling up the longest subsequence table.
            for (int i = 0; i <= str1List.Count; i++)
            {
                for (int j = 0; j <= str2List.Count; j++)
                {
                    if (i == 0 || j == 0)
                        subSeqTable[i, j] = 0;
                    else if (str1List[i - 1].Equals(str2List[j - 1], StringComparison.InvariantCultureIgnoreCase))
                        subSeqTable[i, j] = subSeqTable[i - 1, j - 1] + 1;
                    else
                        subSeqTable[i, j] = Math.Max(subSeqTable[i - 1, j], subSeqTable[i, j - 1]);
                }
            }


            var longestCommonSub = new List<string>();
            var commonSubPosition1 = new List<int>();
            var commonSubPosition2 = new List<int>();

            var c1 = str1List.Count;
            var c2 = str2List.Count;

            while (c1 > 0 && c2 > 0)
            {
                if (str1List[c1 - 1].Equals(str2List[c2 - 1], StringComparison.InvariantCultureIgnoreCase))
                {
                    longestCommonSub.Add(str1List[c1 - 1]);
                    commonSubPosition1.Add(c1 - 1);
                    commonSubPosition2.Add(c2 - 1);
                    c1--;
                    c2--;
                }

                // If not same, then find the larger of two and 
                // go in the direction of larger value 
                else if (subSeqTable[c1 - 1, c2] > subSeqTable[c1, c2 - 1])
                    c1--;
                else
                    c2--;

            }

            //Have to reverse the lists since we are going from bottom up.
            longestCommonSub.Reverse();
            commonSubPosition1.Reverse();
            commonSubPosition2.Reverse();

            return longestCommonSub;
        }

        private List<Word> HandleEdited(List<Word> oldWords, string newTranscript, Guid newVersionId, List<string> newTranscriptList)
        {
            List<Word> newWords = new List<Word>();
            for (int i = 0; i < newTranscriptList.Count; i++)
            {
                newWords.Add(new Word { Term = newTranscriptList[i], Timestamp = oldWords[i].Timestamp, VersionId = newVersionId, Position = i });
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
                        newWords.Add(new Word { Term = newTranscriptList[i], Timestamp = oldWords[j].Timestamp, VersionId = newVersionId, Position = i });
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
                    newWords.Add(new Word { Term = currentNewWord, Timestamp = oldWords[iterateOld].Timestamp, VersionId = newVersionId, Position = i });
                    if (iterateOld != oldWords.Count - 1)
                    {
                        iterateOld++;
                    }
                }
                else
                {
                    if (iterateOld == 0 || iterateOld == oldWords.Count - 1)
                    {
                        newWords.Add(new Word { Term = currentNewWord, Timestamp = oldWords[iterateOld].Timestamp, VersionId = newVersionId, Position = i });
                    }
                    else
                    {
                        newWords.Add(new Word { Term = currentNewWord, Timestamp = oldWords[iterateOld - 1].Timestamp, VersionId = newVersionId, Position = i });
                    }
                }
            }

            return newWords;
        }
    }
}
