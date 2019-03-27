using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RC_SpeechToText.Services
{
    public class ModifyTimeStampService
    {
        public List<Word> ModifyTimestamps(List<Word> oldWords, string oldTranscript, string newTranscript, Guid newVersionId)
        {

            var longestCommonSequence = CommonWords(oldTranscript,newTranscript);
            var newWords2 = CreateNewWords(oldWords, longestCommonSequence.newTranscriptionTerms,
                longestCommonSequence.newTransPosition, longestCommonSequence.oldTransPositions, newVersionId);

            newWords2 = EstimateWords(newWords2);
            //Old logic to be deleted after implementation of new save algorithm
            List<Word> newWords = new List<Word>();

            var newTranscriptNoBr = longestCommonSequence.newTranscriptionTerms;


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

        private CommonSubsequence CommonWords(string oldTranscript, string newTranscript)
        {

            var oldTransList = oldTranscript
                .Replace("<br>", " ")
                .Replace(".", " ")
                .Replace(",", " ")
                .Split(" ")
                .Select(str => str.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var newTransList = newTranscript
               .Replace("<br>", " ")
               .Replace(".", " ")
               .Replace(",", " ")
               .Split(" ")
               .Select(str => str.Trim())
               .Where(x => !string.IsNullOrEmpty(x))
               .ToList();


            var subSeqTable = new int[oldTransList.Count + 1, newTransList.Count + 1];

            //Filling up the longest subsequence table.
            for (int i = 0; i <= oldTransList.Count; i++)
            {
                for (int j = 0; j <= newTransList.Count; j++)
                {
                    if (i == 0 || j == 0)
                        subSeqTable[i, j] = 0;
                    else if (oldTransList[i - 1].Equals(newTransList[j - 1], StringComparison.InvariantCultureIgnoreCase))
                        subSeqTable[i, j] = subSeqTable[i - 1, j - 1] + 1;
                    else
                        subSeqTable[i, j] = Math.Max(subSeqTable[i - 1, j], subSeqTable[i, j - 1]);
                }
            }


            //Saving the positions as well as the words to match them to old timestamps
            var longestCommonSub = new List<string>();
            var commonSubPosition1 = new List<int>();
            var commonSubPosition2 = new List<int>();

            var c1 = oldTransList.Count;
            var c2 = newTransList.Count;

            //Going through the table and saving words/positions
            while (c1 > 0 && c2 > 0)
            {
                if (oldTransList[c1 - 1].Equals(newTransList[c2 - 1], StringComparison.InvariantCultureIgnoreCase))
                {
                    longestCommonSub.Add(oldTransList[c1 - 1]);
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

            var commonSubsequenceInfo = new CommonSubsequence {
                longestCommonSub = longestCommonSub,
                oldTransPositions = commonSubPosition1,
                newTransPosition = commonSubPosition2,
                newTranscriptionTerms = newTransList
            };
            return commonSubsequenceInfo;
        }

        private List<Word> CreateNewWords(List<Word> oldWords, List<string> newTransTerms, List<int> newTransPos, List<int> oldTransPos, Guid newVersionId)
        {
            List<Word> newWords = new List<Word>();
            var counter = 0;
            var counterEdited = 1;
            for (int i = 0; i<newTransTerms.Count; i++)
            {
                //If 1 or no common words between transcripts create all words and estimate timestamps
                if (newTransPos.Count < 2)
                {
                    for (int j = 0; j < newTransTerms.Count; j++)
                    {
                        newWords.Add(new Word { Term = newTransTerms[j], VersionId = newVersionId, Position = j, State = "Estime" });
                    }
                    break;
                }
                //Keeping info from kept words
                if (counter < newTransPos.Count && i == newTransPos[counter])
                {
                    var oldWord = oldWords[oldTransPos[counter]];
                    newWords.Add(new Word { Term = oldWord.Term, Timestamp = oldWord.Timestamp, VersionId = newVersionId, Position = i });
                    counter++;
                    counterEdited = 1;
                }
                //Handle if word edited in beginning of transcript
                else if (counter == 0 && newTransPos[0] == oldTransPos[0])
                {
                    var oldWord = oldWords[counterEdited - 1];
                    newWords.Add(new Word { Term = newTransTerms[i], Timestamp = oldWord.Timestamp, VersionId = newVersionId, Position = i });
                    counterEdited++;
                }
                //Handle if word edited in end of transcript
                else if (counter == (newTransPos.Count) && (newTransTerms.Count - newTransPos[counter - 1]) == (oldWords.Count - oldTransPos[counter - 1]))
                {
                    var oldWord = oldWords[oldTransPos[counter - 1] + counterEdited];
                    newWords.Add(new Word { Term = newTransTerms[i], Timestamp = oldWord.Timestamp, VersionId = newVersionId, Position = i });
                    counterEdited++;
                }
                //Handle if word edited in middle of transcript
                else if (counter > 0 && counter < (newTransPos.Count) && (newTransPos[counter] - newTransPos[counter - 1]) > 1 && (newTransPos[counter] - newTransPos[counter - 1]) == (oldTransPos[counter] - oldTransPos[counter - 1]))
                {
                    var oldWord = oldWords[oldTransPos[counter - 1] + counterEdited];
                    newWords.Add(new Word { Term = newTransTerms[i], Timestamp = oldWord.Timestamp, VersionId = newVersionId, Position = i });
                    counterEdited++;
                }
                else
                {
                    newWords.Add(new Word { Term = newTransTerms[i], VersionId = newVersionId, Position = i, State = "Estime" });
                }

            }

            return newWords;
        }

        private List<Word> EstimateWords(List<Word> newWords)
        {
            //Will save each span of words that need to be estimated
            var positions = new List<List<int>>();
            positions = getSpanPositions(newWords);
            return GenerateTimeStamps(newWords,positions);
                       
        }

        private List<List<int>> getSpanPositions(List<Word> newWords)
        {

            var positions = new List<List<int>>();

            var inSpan = false;
            var spanCounter = 0;

            //Taking all words with "estime" since we have to re-estimate old words to keep it consistent
            for (int i = 0; i < newWords.Count; i++)
            {
                if (newWords[i].State != null && newWords[i].State.Equals("Estime") && !inSpan)
                {
                    positions.Add(new List<int>());
                    positions[spanCounter].Add(i);
                    inSpan = true;
                }
                else if (newWords[i].State != null && newWords[i].State.Equals("Estime") && inSpan)
                {
                    positions[spanCounter].Add(i);
                }
                else
                {
                    if (inSpan)
                        spanCounter++;
                    inSpan = false;
                   
                }
            }

            return positions;
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
