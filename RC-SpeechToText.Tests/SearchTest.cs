using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RC_SpeechToText.Models;
using RC_SpeechToText.Models.DTO.Outgoing;
using RC_SpeechToText.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RC_SpeechToText.Tests
{
    public class SearchTest
    {
        [Fact]
        public async Task TestSearchByDescriptionAndTitle()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Id = Guid.NewGuid(), Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Id = Guid.NewGuid(), Email = "reviewer@email.com", Name = "testReviewer" };
            List<File> files = new List<File>();

            var automatedFlag = Enum.GetName(typeof(FileFlag), 0);

            //Populating our files table
            files.Add( new File { Title = "title1", Description = "This is a description test", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id });
            files.Add( new File { Title = "title test", Description = "2nd description", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id });
            files.Add( new File { Title = "title2", Description = "3rd description", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id });
            files.Add( new File { Title = "title3", Description = "Random word in description btgjsp", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id });
            files.Add( new File { Title = "title4", Description = "4th description", DateAdded = DateTime.Now, Flag = automatedFlag, UserId = user.Id, ReviewerId = reviewer.Id });

            //AddAsync File to database
            await context.File.AddRangeAsync(files);
            await context.SaveChangesAsync();

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());
			
            // Act
            //Searching for title1
            var filesTitle1 = SearchService.SearchDescriptionAndTitle(files, "title1");
            Assert.NotNull(filesTitle1);

            //Checking if only file returned was the one with title : title1
            Assert.Equal("title1", filesTitle1[0].Title);
            Assert.Single(filesTitle1);

            //Searching for btgjsp in description
            var filesBtgjsp = SearchService.SearchDescriptionAndTitle(files, "btgjsp");
            Assert.NotNull(filesBtgjsp);

            //Checking if only file returned was the one with title : title1
            Assert.Equal("title3", filesBtgjsp[0].Title);
            Assert.Single(filesBtgjsp);

            //Searching for descriptions containing word "Description"
            var filesDescription = SearchService.SearchDescriptionAndTitle(files, "description");
            Assert.NotNull(filesDescription);

            //Checking if all files are returned since they all contain keyword description
            Assert.Equal(5 , filesDescription.Count);
        }

        [Fact]
        public async Task TimeStampsOfWordsSearchInTranscript()
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

            var version = new Models.Version { FileId = file.Id, Active = true, Transcription = transcript };

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
            words.Add(new Word { Term = "Six", Timestamp = "\"7.000s\"", VersionId = version.Id, Position = 6 });
            words.Add(new Word { Term = "Sept", Timestamp = "\"8.000s\"", VersionId = version.Id, Position = 7 });

            //Adding the words and their timestamps in the database
            foreach (var word in words)
            {
                await context.Word.AddAsync(word);
                await context.SaveChangesAsync();
            }

            

            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());

            var outSearchDTOExisting = new OutSearchTranscriptDTO { VersionId = version.Id , SearchTerms = "Quatre Cinq Six"};
            var outSearchDTODuplicate = new OutSearchTranscriptDTO { VersionId = version.Id , SearchTerms = "Six"};
            var outSearchDTONonExisting = new OutSearchTranscriptDTO { VersionId = version.Id, SearchTerms = "Seven" };


            // Act
            var transcriptionService = new TranscriptionService(context, config.Value);
            var resultTimestampsExisting = await transcriptionService.SearchTranscript(outSearchDTOExisting);
            var resultTimestampsDuplicate = await transcriptionService.SearchTranscript(outSearchDTODuplicate);
            var resultTimestampsNonExisting = await transcriptionService.SearchTranscript(outSearchDTONonExisting);

            // Assert
            //Checking if it returns the right timeframe for the searched terms
            Assert.Equal("0:00:04-0:00:06", resultTimestampsExisting);
            Assert.Equal("0:00:06-0:00:06, 0:00:07-0:00:07", resultTimestampsDuplicate);
            Assert.Equal("", resultTimestampsNonExisting);

           

        }
    }
}
