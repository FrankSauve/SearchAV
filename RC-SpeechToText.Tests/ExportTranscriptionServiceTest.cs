using Microsoft.VisualStudio.TestTools.UnitTesting;
using RC_SpeechToText.Infrastructure;
using RC_SpeechToText.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC_SpeechToText.Tests
{
	[TestClass]
	class ExportTranscriptionServiceTest
	{

		private readonly string transcription = "Some transcription in here";

		[TestMethod]
		public void TestSuccessfulCreateWordDocument()
		{
			var wordRepo = new WordRepository();
			var result = wordRepo.CreateWordDocument(transcription);

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void TestEmptyTranscriptCreateWordDocument()
		{
			var wordRepo = new WordRepository();
			var result = wordRepo.CreateWordDocument("");

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TestSuccessfulCreateGoogleDocument()
		{
			var googleDocRepo = new GoogleDocumentRepository();
			var result = googleDocRepo.CreateGoogleDocument(transcription);

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void TestEmptyTranscriptCreateGoogleDocument()
		{
			var googleDocRepo = new GoogleDocumentRepository();
			var result = googleDocRepo.CreateGoogleDocument(null);

			Assert.IsFalse(result);
		}
	}
}
