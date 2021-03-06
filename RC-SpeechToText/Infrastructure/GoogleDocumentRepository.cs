﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using RC_SpeechToText.Utils;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RC_SpeechToText.Infrastructure
{
    public class GoogleDocumentRepository
	{
		public bool CreateGoogleDocument(string transcription, string fileTitle)
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
						Title = fileTitle
					};

					DocumentsResource.CreateRequest createRequest = service.Documents.Create(newDocument);
					Document createdDocument = createRequest.Execute();

					DocumentsResource.BatchUpdateRequest batchUpdate = service.Documents.BatchUpdate(GenerateGoogleDocText(transcription.ClearHTMLTag()), createdDocument.DocumentId);
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
	}
}
