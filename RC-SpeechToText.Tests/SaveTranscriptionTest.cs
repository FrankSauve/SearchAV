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
        //[Fact]
        //public async Task TestSaveEditedTranscript()
        //{

        //    var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;

        //    var context = new SearchAVContext(options);

        //    string transcript = "Transcription";

        //    //Add File to database
        //    File f = new File { Title = "title", DateAdded = DateTime.Now, Flag = "Automatisé" };
        //    context.File.Add(f);
        //    int fileId = context.SaveChanges();

        //    //Add Version to database
        //    Version v = new Version { FileId = fileId, Active = true};
        //    v.Transcription = transcript;
        //    context.Version.Add(v);
        //    int versionId = context.SaveChanges();

        //    //Retrieve added Version
        //    v = context.Version.Find(versionId);

        //    string newTranscription = "New Transcription";

        //    var mock = new Mock<ILogger<SaveEditedTranscriptController>>();
        //    ILogger<SaveEditedTranscriptController> logger = mock.Object;
        //    logger = Mock.Of<ILogger<SaveEditedTranscriptController>>();

        //    var controller = new SaveEditedTranscriptController(context, logger);

        //    await controller.SaveEditedTranscript(v.Id, newTranscription);
        //    Assert.NotEqual(v.Transcription, newTranscription);

        //    //Checking new version
        //    Version newVersion = context.Version.Find(versionId + 1);
        //    Assert.Equal(newVersion.Transcription, newTranscription);
        //    Assert.Equal(newVersion.FileId, fileId);
        //    Assert.True(newVersion.Active);

        //    //Checking old version
        //    v = context.Version.Find(versionId);
        //    Assert.Equal(v.Transcription, transcript);
        //    Assert.Equal(v.FileId, newVersion.FileId);
        //    Assert.False(v.Active);

        //    //Checking corresponding file
        //    f = context.File.Find(fileId);
        //    Assert.Equal("Edité", f.Flag);

        //}
    }
}
