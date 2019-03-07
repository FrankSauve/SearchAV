using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using System.Threading.Tasks;
using System;
using Version = RC_SpeechToText.Models.Version;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace RC_SpeechToText.Tests
{
    
    public class SaveTranscriptionTest
    {
        [Fact]
        public async Task TestSaveEditedTranscript()
        {

            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Transcription";

            var user = new User {Id = 1, Email = "user@email.com", Name = "testUser" };
            var reviewer = new User {Id = 2, Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id };

            //AddAsync File to database
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();
            

            var version = new Version { FileId = file.Id, Active = true, Transcription = transcript };

            //AddAsync Version to database
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();
            
            string editTranscription = "Test Edit Transcription";
            string reviewTranscription = "Test Review Transcription";

            var mock = new Mock<ILogger<TranscriptionController>>();
            ILogger<TranscriptionController> logger = mock.Object;
            logger = Mock.Of<ILogger<TranscriptionController>>();

            var controller = new TranscriptionController(context, logger);

            //Editing file
            await controller.SaveTranscript(user.Id, version.Id, editTranscription);
            Assert.NotEqual(version.Transcription, editTranscription);

            //Checking edited version
            Version editedVersion = context.Version.Find(version.Id + 1);
            Assert.Equal(editedVersion.Transcription, editTranscription);
            Assert.Equal(editedVersion.FileId, file.Id);
            Assert.True(editedVersion.Active);

            //Checking corresponding file
            file = context.File.Find(file.Id);
            Assert.Equal("Edité", file.Flag);

            //----------------------------------------------------------------------------------------------------------------
            //Review file
            await controller.SaveTranscript(reviewer.Id, editedVersion.Id, reviewTranscription);
            Assert.NotEqual(editedVersion.Transcription, reviewTranscription);

            //Checking new version
            Version reviewedVersion = context.Version.Find(editedVersion.Id + 1);
            Assert.Equal(reviewedVersion.Transcription, reviewTranscription);
            Assert.Equal(reviewedVersion.FileId, file.Id);
            Assert.True(reviewedVersion.Active);
            Assert.False(editedVersion.Active);

            //Checking corresponding file
            file = context.File.Find(file.Id);
            Assert.Equal("Révisé", file.Flag);

            //-----------------------------------------------------------------------------------------------------------------
            //Checking old version
            version = context.Version.Find(version.Id);
            Assert.Equal(version.Transcription, transcript);
            Assert.Equal(version.FileId, editedVersion.FileId);
            Assert.Equal(version.FileId, reviewedVersion.FileId);
            Assert.False(version.Active);

        }

		[Fact]
		public async void DownloadTranscriptTest()
		{
			var context = new SearchAVContext(DbContext.CreateNewContextOptions());

			var versionMock = new List<Version>
			{
				new Version { Active = false, DateModified = null, FileId = 119, Id = 1, Transcription = "version 1", UserId = 1},
				new Version { Active = true, DateModified = null, FileId = 119, Id = 2, Transcription = "version <br> 2", UserId = 1},
				new Version { Active = true, DateModified = null, FileId = 120, Id = 3, Transcription = "version <br> 3 <br>", UserId = 2}
			};

			var mock = new Mock<ILogger<TranscriptionController>>();
			ILogger<TranscriptionController> logger = mock.Object;
			logger = Mock.Of<ILogger<TranscriptionController>>();

			

			var fileId = 119;
			var version = versionMock.Where(v => v.FileId == fileId).Where(v => v.Active == true).SingleOrDefault();
			Assert.Equal(version.FileId, fileId);
			Assert.True(version.Active);

			var transcript = version.Transcription.Replace("<br>", "\n");
			Assert.DoesNotContain(transcript, "<br>");
			Assert.Contains("\n", transcript);

			//AddAsync Version to database
			versionMock.ForEach(async x => await context.Version.AddAsync(x));
			await context.SaveChangesAsync();

			var documentType = "fake type";

			var controller = new TranscriptionController(context, logger);
			var result = await controller.DownloadTranscript(documentType, fileId);
			Assert.IsType<BadRequestObjectResult>(result);
		}

	}
}
