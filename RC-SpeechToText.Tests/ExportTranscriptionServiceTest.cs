using Microsoft.VisualStudio.TestTools.UnitTesting;
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
		private readonly ExportTranscriptionService testService = new ExportTranscriptionService();

		[TestMethod]
		public void TestSuccessfulCreateWordDocument()
		{
			var result = testService.CreateWordDocument(transcription);

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void TestEmptyTranscriptCreateWordDocument()
		{
			var result = testService.CreateWordDocument("");

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TestSuccessfulCreateGoogleDocument()
		{
			var result = testService.CreateGoogleDocument(transcription);

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void TestEmptyTranscriptCreateGoogleDocument()
		{
			var result = testService.CreateGoogleDocument(null);

			Assert.IsFalse(result);
		}
	}
}
