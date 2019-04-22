using Microsoft.EntityFrameworkCore;
using Xunit;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using System.Threading.Tasks;
using System;
using Version = RC_SpeechToText.Models.Version;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RC_SpeechToText.Models.DTO.Incoming;
using RC_SpeechToText.Models.DTO.Outgoing;

namespace RC_SpeechToText.Tests
{
    
    public class SaveTranscriptionTest
    {
        [Fact]
        public async Task TestSaveEditedTranscript()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Transcription";

            var automatedFlag = Enum.GetName(typeof(FileFlag), 0);
            var editedFlag = Enum.GetName(typeof(FileFlag), 1);
            var reviewedFlag = Enum.GetName(typeof(FileFlag), 2);

            var user = new User {Id = Guid.NewGuid(), Email = "user@email.com", Name = "testUser" };
            var reviewer = new User {Id = Guid.NewGuid(), Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id, Duration = "00:00:30"};
            
            //AddAsync File to database
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();
            
            var version = new Version { FileId = file.Id, Active = true, Transcription = transcript };
            
            //AddAsync Version to database
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();

            //Creating words and their timestamps for the original transcript
            List<Word> words = new List<Word>
            {
                new Word { Term = "Transcription", Timestamp = "\"1.000s\"", VersionId = version.Id }
            };
            await context.Word.AddAsync(words[0]);
            await context.SaveChangesAsync();

            string editTranscription = "Test Edit Transcription";
            string reviewTranscription = "Test Review Transcription";

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());

			// Act
            var transcriptionService = new TranscriptionService(context, config.Value);

            await transcriptionService.SaveTranscript(user.Email, version.Id, editTranscription);

            // Assert
            Assert.NotEqual(version.Transcription, editTranscription);

            //Checking edited version
            Version editedVersion = await context.Version.Where(v => v.FileId == file.Id).LastAsync();
            Assert.Equal(editedVersion.Transcription, editTranscription);
            Assert.Equal(editedVersion.FileId, file.Id);
            Assert.True(editedVersion.Active);

            //----------------------------------------------------------------------------------------------------------------
            //Review file
            await transcriptionService.SaveTranscript(reviewer.Email, editedVersion.Id, reviewTranscription);

            Assert.NotEqual(editedVersion.Transcription, reviewTranscription);

            //Checking new version
            Version reviewedVersion = await context.Version.Where(v => v.FileId == file.Id).LastAsync();
            Assert.Equal(reviewedVersion.Transcription, reviewTranscription);
            Assert.Equal(reviewedVersion.FileId, file.Id);
            Assert.True(reviewedVersion.Active);
            Assert.False(editedVersion.Active);

            //-----------------------------------------------------------------------------------------------------------------
            //Checking old version
            version = context.Version.Find(version.Id);
            Assert.Equal(version.Transcription, transcript);
            Assert.Equal(version.FileId, editedVersion.FileId);
            Assert.Equal(version.FileId, reviewedVersion.FileId);
            Assert.False(version.Active);
        }

        [Fact]
        public async Task TestAddWordsTimeStamps()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Un Deux Trois Quatre Cinq Six Sept";

            var automatedFlag = Enum.GetName(typeof(FileFlag), 0);

            var user = new User { Id = Guid.NewGuid(), Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Id = Guid.NewGuid(), Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id, Duration = "00:00:09" };

            //AddAsync File to database
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();

            var version = new Version { FileId = file.Id, Active = true, Transcription = transcript };

            //AddAsync Version to database
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();

            //Creating words and their timestamps for the original transcript
            List<Word> words = new List<Word>();
            words.Add(new Word { Term = "Un", Timestamp = "\"1.000s\"", VersionId = version.Id, Position = 0 });
            words.Add(new Word { Term = "Deux", Timestamp = "\"2.000s\"", VersionId = version.Id, Position = 1 });
            words.Add(new Word { Term = "Trois", Timestamp = "\"3.000s\"", VersionId = version.Id, Position = 2 });
            words.Add(new Word { Term = "Quatre", Timestamp = "\"4.000s\"", VersionId = version.Id, Position = 3 });
            words.Add(new Word { Term = "Cinq", Timestamp = "\"5.000s\"", VersionId = version.Id, Position = 4 });
            words.Add(new Word { Term = "Six", Timestamp = "\"6.000s\"", VersionId = version.Id, Position = 5 });
            words.Add(new Word { Term = "Sept", Timestamp = "\"7.000s\"", VersionId = version.Id, Position = 6 });

            //Adding the words and their timestamps in the database
            foreach (var word in words)
            {
                await context.Word.AddAsync(word);
                await context.SaveChangesAsync();
            }

            string addWordsTranscription = "Un Deux DeuxDeux Trois Quatre Cinq Six Sept Huit";// Added DeuxDeux and Huit

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());
			
            // Act
            var transcriptionService = new TranscriptionService(context, config.Value);
            await transcriptionService.SaveTranscript(user.Email, version.Id, addWordsTranscription);

            //Get the words for new version
            Version newVersion = await context.Version.Where(v => v.FileId == file.Id).LastAsync();
            List<Word> addedWords = await context.Word.Where(w => w.VersionId == newVersion.Id).OrderBy(w => w.Position).ToListAsync();

            // Assert
            //Check if each words related to new transcription has right timestamp 
            //(They should have the timestamp of the word that precedes them
            Assert.Equal("\"2.500s\"", addedWords[2].Timestamp);
            Assert.Equal("\"8.000s\"", addedWords[8].Timestamp);
        }

        [Fact]
        public async Task TestDeleteWordsTimeStamps()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Un Deux Trois Quatre Cinq Six Sept";

            var automatedFlag = Enum.GetName(typeof(FileFlag), 0);

            var user = new User { Id = Guid.NewGuid(), Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Id = Guid.NewGuid(), Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id, Duration = "00:00:08" };

            //AddAsync File to database
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();

            var version = new Version { FileId = file.Id, Active = true, Transcription = transcript };

            //AddAsync Version to database
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();

            //Creating words and their timestamps for the original transcript
            List<Word> words = new List<Word>();
            words.Add(new Word { Term = "Un", Timestamp = "\"1.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Deux", Timestamp = "\"2.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Trois", Timestamp = "\"3.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Quatre", Timestamp = "\"4.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Cinq", Timestamp = "\"5.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Six", Timestamp = "\"6.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Sept", Timestamp = "\"7.000s\"", VersionId = version.Id });

            //Adding the words and their timestamps in the database
            foreach (var word in words)
            {
                await context.Word.AddAsync(word);
                await context.SaveChangesAsync();
            }

            string addWordsTranscription = "Deux Trois Quatre Six";// Removed word Un/Cinq/Sept

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());

            // Act
            var transcriptionService = new TranscriptionService(context, config.Value);
            await transcriptionService.SaveTranscript(user.Email, version.Id, addWordsTranscription);

            //Get the words for new version
            Version newVersion = await context.Version.Where(v => v.FileId == file.Id).LastAsync();
            List<Word> addedWords = await context.Word.Where(w => w.VersionId == newVersion.Id).OrderBy(w => w.Position).ToListAsync();

            // Assert
            //Check if each words related to new transcription has right timestamp 
            //(They should have the same timestamps with the removed words gone)
            Assert.Equal("\"2.000s\"", addedWords[0].Timestamp);
            Assert.Equal("\"3.000s\"", addedWords[1].Timestamp);
            Assert.Equal("\"4.000s\"", addedWords[2].Timestamp);
            Assert.Equal("\"6.000s\"", addedWords[3].Timestamp);
        }

        [Fact]
        public async Task TestModifyWordsTimeStamps()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Un Deux Trois Quatre Cinq Six Sept";

            var automatedFlag = Enum.GetName(typeof(FileFlag), 0);

            var user = new User { Id = Guid.NewGuid(), Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Id = Guid.NewGuid(), Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id, Duration = "00:00:08" };

            //AddAsync File to database
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();

            var version = new Version { FileId = file.Id, Active = true, Transcription = transcript };

            //AddAsync Version to database
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();

            //Creating words and their timestamps for the original transcript
            List<Word> words = new List<Word>();
            words.Add(new Word { Term = "Un", Timestamp = "\"1.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Deux", Timestamp = "\"2.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Trois", Timestamp = "\"3.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Quatre", Timestamp = "\"4.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Cinq", Timestamp = "\"5.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Six", Timestamp = "\"6.000s\"", VersionId = version.Id });
            words.Add(new Word { Term = "Sept", Timestamp = "\"7.000s\"", VersionId = version.Id });

            //Adding the words and their timestamps in the database
            foreach (var word in words)
            {
                await context.Word.AddAsync(word);
                await context.SaveChangesAsync();
            }

            string addWordsTranscription = "Uno Deux Trois Quatre Cinqo Six Seven";// Modified word Un/Cinq/Sept

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());

            // Act
            var transcriptionService = new TranscriptionService(context, config.Value);
            await transcriptionService.SaveTranscript(user.Email, version.Id, addWordsTranscription);

            //Get the words for new version
            Version newVersion = await context.Version.Where(v => v.FileId == file.Id).LastAsync();
            List<Word> addedWords = await context.Word.Where(w => w.VersionId == newVersion.Id).OrderBy(w => w.Position).ToListAsync();

            // Assert
            //Check if each words related to new transcription has right timestamp 
            //(They should have the same timestamps with the modified words keeping their)
            Assert.Equal("\"1.000s\"", addedWords[0].Timestamp);
            Assert.Equal("\"5.000s\"", addedWords[4].Timestamp);
            Assert.Equal("\"7.000s\"", addedWords[6].Timestamp);
        }

        [Fact]
		public async void DownloadTranscriptTest()
		{
            // Arrange
			var context = new SearchAVContext(DbContext.CreateNewContextOptions());

			var versionMock = new List<Version>
			{
				new Version { Active = false, DateModified = null, FileId = Guid.Parse("36b36fb0-e51e-4033-7337-08d6ac0a018e"), Id = Guid.NewGuid(), Transcription = "version 1", UserId = Guid.NewGuid()},
				new Version { Active = true, DateModified = null, FileId = Guid.Parse("36b36fb0-e51e-4033-7337-08d6ac0a018e"), Id = Guid.NewGuid(), Transcription = "version <br> 2", UserId = Guid.NewGuid()},
				//new Version { Active = true, DateModified = null, FileId = Guid.Parse("36b36fb0-e51e-4033-7337-08d6ac0a018e"), Id = Guid.NewGuid(), Transcription = "version <br> 3 <br>", UserId = Guid.NewGuid()}
			};

			var fileId = Guid.Parse("36b36fb0-e51e-4033-7337-08d6ac0a018e");
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

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());
			
            // Act
            var transcriptionService = new TranscriptionService(context, config.Value);
            var outDTO = new OutDownloadTranscriptDTO { FileId = fileId, DocumentType = documentType };
            var errMsg = await transcriptionService.PrepareDownload(outDTO);
            Assert.Equal("Error while trying to download transcription", errMsg);
        }

	}
}
