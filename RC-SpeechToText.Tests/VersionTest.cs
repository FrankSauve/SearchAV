using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RC_SpeechToText.Tests
{
    public class VersionTest
    {
        /// <summary>
        /// Test fetching all versions
        /// Should return a list of versions
        /// </summary>
        [Fact]
        public async Task TestGetAll()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
            var context = new SearchAVContext(options);

            await context.Version.AddRangeAsync(Enumerable.Range(1, 20).Select(t => new Models.Version { Transcription = "transcription", FileId = 1, UserId = 1}));
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new VersionController(context, logger);

            //Act
            var result = await controller.Index();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Models.Version>>(okResult.Value);
            Assert.True(returnValue.Count() == 20);
        }

        /// <summary>
        /// Test fetching one File by Id
        /// Should return a list of versions
        /// </summary>
        [Fact]
        public async Task TestGetByFileId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
            var context = new SearchAVContext(options);

            // Remove all versions from db
            var versions = await context.Version.ToListAsync();
            context.Version.RemoveRange(versions);
            await context.SaveChangesAsync();

            // Add file with title testFile
            var version = new Models.Version { Transcription = "transcription", FileId = 1, UserId = 1 };
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new VersionController(context, logger);

            // Act
            var result = await controller.GetbyFileId(version.FileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Models.Version>>(okResult.Value);
        }
    }
}
