using Microsoft.AspNetCore.Mvc;
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
    public class WordTest
    {
        [Fact]
        public async Task TestGetByVersionId()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            // Add 20 words to DB
            await context.Word.AddRangeAsync(Enumerable.Range(1, 20).Select(t => new Word { Term = "test", VersionId = 1 }));
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            // Act
            var controller = new WordController(context, logger);
            var result = await controller.GetByVersionId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Word>>(okResult.Value);
            Assert.Equal(20, returnValue.Count());
        }
    }
}
