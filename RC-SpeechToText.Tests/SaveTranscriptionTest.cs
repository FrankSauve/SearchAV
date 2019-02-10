using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using System.Threading.Tasks;
using System;
using Version = RC_SpeechToText.Models.Version;

namespace RC_SpeechToText.Tests
{
    
    public class SaveTranscriptionTest
    {
        [Fact]
        public async Task TestSaveEditedTranscript()
        {

            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Transcription";

            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = "Automatisé" };

            //AddAsync File to database
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();
            

            var version = new Version { FileId = file.Id, Active = true, Transcription = transcript };

            //AddAsync Version to database
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();
            
            string newTranscription = "New Transcription";

            var mock = new Mock<ILogger<SaveEditedTranscriptController>>();
            ILogger<SaveEditedTranscriptController> logger = mock.Object;
            logger = Mock.Of<ILogger<SaveEditedTranscriptController>>();

            var controller = new SaveEditedTranscriptController(context, logger);

            await controller.SaveEditedTranscript(version.Id, newTranscription);
            Assert.NotEqual(version.Transcription, newTranscription);

            //Checking new version
            Version newVersion = context.Version.Find(version.Id + 1);
            Assert.Equal(newVersion.Transcription, newTranscription);
            Assert.Equal(newVersion.FileId, file.Id);
            Assert.True(newVersion.Active);

            //Checking old version
            version = context.Version.Find(version.Id);
            Assert.Equal(version.Transcription, transcript);
            Assert.Equal(version.FileId, newVersion.FileId);
            Assert.False(version.Active);

            //Checking corresponding file
            file = context.File.Find(file.Id);
            Assert.Equal("Edité", file.Flag);
            
        }
    }
}
