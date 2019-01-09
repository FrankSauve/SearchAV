using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Controllers.Transcription;
using RC_SpeechToText.Models;
using System;

namespace RC_SpeechToText.Tests
{
    [TestClass]
    public class SaveTranscriptionTest
    {
        [TestMethod]
        public void TestSave()
        {

            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;

            var context = new SearchAVContext(options);

            //Add video to database
            Video v = new Video();
            v.Transcription = "Transcription";
            context.Video.Add(v);
            context.SaveChanges();

            //Retrieve newly added video
            v = context.Video.Find(1);

            //string trans1 = v.Transcription;
            string newTranscription = "New Transcription";

            var mock = new Mock<ILogger<SavingTranscriptController>>();
            ILogger<SavingTranscriptController> logger = mock.Object;
            logger = Mock.Of<ILogger<SavingTranscriptController>>();

            var controller = new SavingTranscriptController(context, logger);

            controller.SaveTranscript(v.VideoId +"", v.Transcription, newTranscription);
            Assert.AreEqual(v.Transcription, newTranscription);

            //Checking if the transcription has been saved in DB
            v = context.Video.Find(1);
            Assert.AreEqual(v.Transcription, newTranscription);

        }
    }
}
