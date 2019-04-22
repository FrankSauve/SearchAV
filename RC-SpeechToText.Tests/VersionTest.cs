using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

            await context.Version.AddRangeAsync(Enumerable.Range(1, 20).Select(t => new Models.Version { Transcription = "transcription", FileId = Guid.NewGuid(), UserId = Guid.NewGuid()}));
            await context.SaveChangesAsync();
			
            //Act
            var versionService = new VersionService(context);
            var result = await versionService.GetAllVersions();

            // Assert
            Assert.IsType<List<Models.Version>>(result);
            Assert.Equal(20, result.Count());
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
            var version = new Models.Version { Transcription = "transcription", FileId = Guid.NewGuid(), UserId = Guid.NewGuid() };
            await context.Version.AddAsync(version);
            await context.SaveChangesAsync();
			
            // Act
            var versionService = new VersionService(context);
            var result = await versionService.GetVersionByFileId(version.FileId);

            // Assert
            Assert.IsType<List<Models.Version>>(result);
            Assert.Single(result);
        }
    }
}
