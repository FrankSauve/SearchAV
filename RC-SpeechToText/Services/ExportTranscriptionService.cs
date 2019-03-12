using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using RC_SpeechToText.Infrastructure;
using RC_SpeechToText.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Word = Microsoft.Office.Interop.Word;

namespace RC_SpeechToText.Services
{
    public class ExportTranscriptionService
	{
		public bool CreateSRTDocument(string transcription, List<Models.Word> words)
		{
			//get each paragraph. Remove all empty string (where <br> are present). Trim the strings
			var paragraph = transcription.Split("\n").ToList().RemoveEmptyString().Select(str => str.Trim()).ToList();
			var timestamps = new List<string>();
			//Count all the word that have been already passed through. => O(logN^2)
			var wordPassed = 0;

			foreach(string p in paragraph)
			{
				var paragraphWords = p.Split(" ");
				timestamps.AddRange(GetParagraphTimestamp(paragraphWords, words.Skip(wordPassed).ToList()));
				wordPassed += paragraphWords.Count() - 1;
			}

			var streamIO = new IOInfrastructure();
			streamIO.GenerateSRTFile(paragraph, timestamps);

			return true;
		}

		private List<string> GetParagraphTimestamp(string[] paragraph, List<Models.Word> words)
		{
			//Look for the first instance where the paragraph word match & the word db match
			var firstWord = words.Find(x => x.Term == paragraph.First());
			//Same but for the last word
			var lastWord = words.Find(x => x.Term == paragraph.Last().RemovePunctuation());
			return new List<string>
			{
				//Save the time stamp
				FormatTimestamp(firstWord.Timestamp),
				FormatTimestamp(lastWord.Timestamp)
			};
		}

		private string FormatTimestamp(string timestamp)
		{
			//getting this "\"4.600s\"", should be this 00:00:04,600
			var temp = string.Join(string.Empty, Regex.Matches(timestamp, @"\d+").OfType<Match>().Select(m => m.Value));

			for (int i = temp.Count(); i < 9; i++) //there is always 9 numbers
			{
				temp = "0" + temp;
			}

			temp = temp.Insert(2, ":").Insert(5, ":").Insert(8, ",");
			return temp;
		}
	}
}
