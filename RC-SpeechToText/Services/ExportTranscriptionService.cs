using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static Google.Apis.Docs.v1.DocsService;
using Word = Microsoft.Office.Interop.Word;

namespace RC_SpeechToText.Services
{
    public class ExportTranscriptionService
	{
		public bool CreateWordDocument(string transcription)
		{
			try
			{
				Word.Application app = new Word.Application();
				Word.Document doc = app.Documents.Add();

				object start = 0;
				object end = 0;

				Word.Range range = doc.Range(ref start, ref end);
				range.Text = transcription;
				range.Select();

				doc.Save();
				return true;
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
				var credential = GetUserCredential();

				return true;
			}
			catch
			{
				return false;
			}
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
	}
}
