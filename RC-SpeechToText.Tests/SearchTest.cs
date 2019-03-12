using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RC_SpeechToText.Tests
{
    public class SearchTest
    {
        [Fact]
        public async Task TestSearchByDescriptionAndTitle()
        {

            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Id = 1, Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Id = 2, Email = "reviewer@email.com", Name = "testReviewer" };
            List<File> files = new List<File>();

            //Populating our files table
            files.Add( new File { Title = "title1", Description = "This is a description test", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id });
            files.Add( new File { Title = "title test", Description = "2nd description", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id });
            files.Add( new File { Title = "title2", Description = "3rd description", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id });
            files.Add( new File { Title = "title3", Description = "Random word in description btgjsp", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id });
            files.Add( new File { Title = "title4", Description = "4th description", DateAdded = DateTime.Now, Flag = "Automatisé", UserId = user.Id, ReviewerId = reviewer.Id });

            //AddAsync File to database
            await context.File.AddRangeAsync(files);
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new FileController(context, logger);


            //Searching for title1

            IActionResult awaitResult = await controller.GetFilesByDescriptionAndTitle("title1");
            var okResult = awaitResult as OkObjectResult;
            Assert.NotNull(okResult);

            List<File> filesTitle1 = okResult.Value as List<File>; 
            //Checking if only file returned was the one with title : title1
            Assert.Equal("title1", filesTitle1[0].Title);
            Assert.Single(filesTitle1);

            //Searching for btgjsp in description
            awaitResult = await controller.GetFilesByDescriptionAndTitle("btgjsp");
            okResult = awaitResult as OkObjectResult;
            Assert.NotNull(okResult);

            List<File> filesBtgjsp = okResult.Value as List<File>;
            //Checking if only file returned was the one with title : title1
            Assert.Equal("title3", filesBtgjsp[0].Title);
            Assert.Single(filesBtgjsp);

            //Searching for descriptions containing word "Description"
            awaitResult = await controller.GetFilesByDescriptionAndTitle("description");
            okResult = awaitResult as OkObjectResult;
            Assert.NotNull(okResult);

            List<File> filesDescription = okResult.Value as List<File>;
            //Checking if all files are returned since they all contain keyword description
            Assert.Equal(5 , filesDescription.Count);
            
        }
    }
}
