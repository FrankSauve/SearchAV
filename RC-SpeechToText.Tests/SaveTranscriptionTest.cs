using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using System;
using Version = RC_SpeechToText.Models.Version;

namespace RC_SpeechToText.Tests
{
    [TestClass]
    public class SaveTranscriptionTest
    {
        [TestMethod]
        public async void TestSaveEditedTranscript()
        {

            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;

            var context = new SearchAVContext(options);

            string transcript = "Transcription";

            //Add File to database
            File f = new File { Title = "title", DateAdded = DateTime.Now, Flag = "Automatisé" };
            context.File.Add(f);
            int fileId = context.SaveChanges();

            //Add Version to database
            Version v = new Version { FileId = fileId, Active = true};
            v.Transcription = transcript;
            context.Version.Add(v);
            int versionId = context.SaveChanges();

            //Retrieve newly added Version
            v = context.Version.Find(versionId);

            string newTranscription = "New Transcription";

            var mock = new Mock<ILogger<SaveEditedTranscriptController>>();
            ILogger<SaveEditedTranscriptController> logger = mock.Object;
            logger = Mock.Of<ILogger<SaveEditedTranscriptController>>();

            var controller = new SaveEditedTranscriptController(context, logger);

            await controller.SaveEditedTranscript(v.Id + "", v.Transcription, newTranscription);
            Assert.AreEqual(v.Transcription, newTranscription);

            //Checking new version
            Version newVersion = context.Version.Find(versionId + 1);
            Assert.AreEqual(newVersion.Transcription, newTranscription);
            Assert.AreEqual(newVersion.FileId, fileId);
            Assert.IsTrue(newVersion.Active);

            //Checking old version
            v = context.Version.Find(versionId);
            Assert.AreEqual(v.Transcription, transcript);
            Assert.AreEqual(v.FileId, newVersion.FileId);
            Assert.IsFalse(newVersion.Active);

            //Checking corresponding file
            f = context.File.Find(fileId);
            Assert.AreEqual(f.Flag, "Edité");

        }
    }
}
