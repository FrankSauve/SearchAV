
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RC_SpeechToText.Tests
{
    public class FileTest
    {
        /// <summary>
        /// Test fetching all the files
        /// </summary>
        [Fact]
        public async Task TestGetAllVideos()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
            var context = new SearchAVContext(options);

            await context.File.AddRangeAsync(Enumerable.Range(1, 20).Select(t => new File { Title = "Video " + t, FilePath = "vPath " + t }));
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new FileController(context, logger);

            //Act
            var result = await controller.Index();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<File>>(okResult.Value);
            Assert.True(returnValue.Count() >= 0);

        }

        /// <summary>
        /// Test fetching all the files with the corresponding usernames
        /// </summary>
        [Fact]
        public async Task TestGetAllWithUsernames()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
            var context = new SearchAVContext(options);

            // Add user with username testUser
            var user = new User { Email = "test@email.com", Name = "testUser" };
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            // Remove all file in DB
            var files = await context.File.ToListAsync();
            context.File.RemoveRange(files);
            await context.SaveChangesAsync();

            // Add file with title testFile
            await context.File.AddAsync(new File { Title = "testFile", UserId = user.Id });
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new FileController(context, logger);

            // Act
            var result = await controller.GetAllWithUsernames();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<JsonResult>(okResult.Value);

            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic data = JsonConvert.DeserializeObject<object>(json);

            // Assert that username is testUser
            string username = data.Value.usernames[0];
            Assert.Equal("testUser", username);

            // Assert that title is testFile
            string fileTitle = data.Value.files[0].Title;
            Assert.Equal("testFile", fileTitle);
        }
    }
}
