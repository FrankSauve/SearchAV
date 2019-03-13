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

            var controller = new TranscriptionController(context);

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
            Assert.Equal("Automatisé", file.Flag);

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
            Assert.Equal("Automatisé", file.Flag);

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

            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Un Deux Trois Quatre Cinq Six Sept";

            var user = new User { Id = 1, Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Id = 2, Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id };

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
            foreach(var word in words)
            {
                await context.Word.AddAsync(word);
                await context.SaveChangesAsync();
            }

            string addWordsTranscription = "Un Deux DeuxDeux Trois Quatre Cinq Six Sept Huit";// Added DeuxDeux and Huit
            
            var controller = new TranscriptionController(context);

            //Editing file to add Words
            await controller.SaveTranscript(user.Id, version.Id, addWordsTranscription);

            //Get the words for new version
            List<Word> addedWords = await context.Word.Where(w => w.VersionId == version.Id + 1).OrderBy(w => w.Id).ToListAsync();

            //Check if each words related to new transcription has right timestamp 
            //(They should have the timestamp of the word that precedes them)
            Assert.Equal("\"1.000s\"", addedWords[0].Timestamp);
            Assert.Equal("\"2.000s\"", addedWords[1].Timestamp);
            Assert.Equal("\"2.000s\"", addedWords[2].Timestamp);
            Assert.Equal("\"3.000s\"", addedWords[3].Timestamp);
            Assert.Equal("\"4.000s\"", addedWords[4].Timestamp);
            Assert.Equal("\"5.000s\"", addedWords[5].Timestamp);
            Assert.Equal("\"6.000s\"", addedWords[6].Timestamp);
            Assert.Equal("\"7.000s\"", addedWords[7].Timestamp);
            Assert.Equal("\"7.000s\"", addedWords[8].Timestamp);
            
           
        }

        [Fact]
        public async Task TestDeleteWordsTimeStamps()
        {
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Un Deux Trois Quatre Cinq Six Sept";

            var user = new User { Id = 1, Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Id = 2, Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id };

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

            var controller = new TranscriptionController(context);

            //Editing file to delete words
            await controller.SaveTranscript(user.Id, version.Id, addWordsTranscription);

            //Get the words for new version
            List<Word> addedWords = await context.Word.Where(w => w.VersionId == version.Id + 1).OrderBy(w => w.Id).ToListAsync();

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
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            string transcript = "Un Deux Trois Quatre Cinq Six Sept";

            var user = new User { Id = 1, Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Id = 2, Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "title", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id };

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

            var controller = new TranscriptionController(context);

            //Editing file to modify words
            await controller.SaveTranscript(user.Id, version.Id, addWordsTranscription);

            //Get the words for new version
            List<Word> addedWords = await context.Word.Where(w => w.VersionId == version.Id + 1).OrderBy(w => w.Id).ToListAsync();

            //Check if each words related to new transcription has right timestamp 
            //(They should have the same timestamps with the modified words keeping their)
            Assert.Equal("\"1.000s\"", addedWords[0].Timestamp);
            Assert.Equal("\"2.000s\"", addedWords[1].Timestamp);
            Assert.Equal("\"3.000s\"", addedWords[2].Timestamp);
            Assert.Equal("\"4.000s\"", addedWords[3].Timestamp);
            Assert.Equal("\"5.000s\"", addedWords[4].Timestamp);
            Assert.Equal("\"6.000s\"", addedWords[5].Timestamp);
            Assert.Equal("\"7.000s\"", addedWords[6].Timestamp);
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

			var controller = new TranscriptionController(context);
			var result = await controller.DownloadTranscript(documentType, fileId);
			Assert.IsType<BadRequestObjectResult>(result);
		}

	}
}
