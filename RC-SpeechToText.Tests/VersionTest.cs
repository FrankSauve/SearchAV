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
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            await context.Version.AddRangeAsync(Enumerable.Range(1, 20).Select(t => new Models.Version { Transcription = "transcription", FileId = 1, UserId = 1}));
            await context.SaveChangesAsync();
			
            //Act
            var controller = new VersionController(context);
            var result = await controller.Index();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Models.Version>>(okResult.Value);
            Assert.Equal(20, returnValue.Count());
        }

        /// <summary>
        /// Test fetching one File by Id
        /// Should return a list of versions
        /// </summary>
        [Fact]
        public async Task TestGetByFileId()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            // AddAsync file with title testFile
            var version = new Models.Version { Transcription = "transcription", FileId = 1, UserId = 1 };
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();
			
            // Act
            var controller = new VersionController(context);
            var result = await controller.GetbyFileId(version.FileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Models.Version>>(okResult.Value);
        }
    }
}
