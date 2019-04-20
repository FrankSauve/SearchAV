using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RC_SpeechToText.Models;
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
    }
}
