using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
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
		public bool CreateWordDocument(string transcription)
		{
			try
			{
				if (!transcription.IsNullOrEmpty())
				{
					var wordDocument = GenerateWordDocument(transcription);
					wordDocument.Save();

					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}

		public bool CreateGoogleDocument(string transcription)
		{
			try
			{
				if (!transcription.IsNullOrEmpty())
				{
					var credential = GetUserCredential();

					// Create Google Docs API service.
					var service = new DocsService(new BaseClientService.Initializer()
					{
						HttpClientInitializer = credential,
						ApplicationName = "SearchAV Radio-Canada",
					});

					var newDocument = new Document
					{
						Title = transcription.Substring(0, 10)
					};

					DocumentsResource.CreateRequest createRequest = service.Documents.Create(newDocument);
					Document createdDocument = createRequest.Execute();

					DocumentsResource.BatchUpdateRequest batchUpdate = service.Documents.BatchUpdate(GenerateGoogleDocText(transcription), createdDocument.DocumentId);
					batchUpdate.Execute();

					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}

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

			GenerateSRTFile(paragraph, timestamps);

			return true;
		}

		private void GenerateSRTFile(List<string> paragraph, List<string> timestamps)
		{
			//TODO: Find a way to prompt the user on the file path
			TextWriter tw = new StreamWriter(paragraph[0].Substring(0, 10) + ".srt");

			//Write each line as follow:
			//1 (the paragraph count)
			//00:00:00,000 --> 00:00:00,000
			//Paragraph
			//-blank line-
			var paragraphCount = 0;
			foreach(string p in paragraph)
			{
				tw.WriteLine(paragraphCount.ToString());
				tw.WriteLine(timestamps[paragraphCount] + " --> " + timestamps[paragraphCount + 1]);
				tw.WriteLine(paragraph[paragraphCount]);
				tw.WriteLine("");
				paragraphCount++;
			}
			tw.Close();

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

		private UserCredential GetUserCredential()
		{
			UserCredential credential;
			string[] Scopes = { DocsService.Scope.Documents };

			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				string credPath = "token.json";

				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
			}

			return credential;
		}

		private BatchUpdateDocumentRequest GenerateGoogleDocText(string transcription)
		{
			var endOfSegment = new EndOfSegmentLocation();
			var inserttextRequest = new InsertTextRequest
			{
				Text = transcription,
				EndOfSegmentLocation = endOfSegment
			};

			var request = new Request
			{
				InsertText = inserttextRequest
			};

			var requestList = new List<Request>()
				{
					request
				};

			var updateRequest = new BatchUpdateDocumentRequest
			{
				Requests = requestList
			};

			return updateRequest;
		}

		private Word.Document GenerateWordDocument(string transcription)
		{
			Word.Application app = new Word.Application();
			Word.Document doc = app.Documents.Add();

			object start = 0;
			object end = 0;

			Word.Range range = doc.Range(ref start, ref end);
			range.Text = transcription;
			range.Select();

			return doc;
		}
	}
}
