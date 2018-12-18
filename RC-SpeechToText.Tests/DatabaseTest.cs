
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

namespace RC_SpeechToText.Tests
{
    public class DatabaseTest
    {
        [Fact]
        public void GettAllVideos()
        {
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;

            var context = new SearchAVContext(options);
            context.Video.AddRange(Enumerable.Range(1, 20).Select(t => new Video { Title = "Video " + t, VideoPath = "vPath " + t, Transcription = "tPath " + t }));
            context.SaveChanges();

            var mock = new Mock<ILogger<VideoController>>();
            ILogger<VideoController> logger = mock.Object;

            //or use this short equivalent 
            logger = Mock.Of<ILogger<VideoController>>();

            var controller = new VideoController(context, logger);

            //Act
            var result = controller.Index();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Video>>(okResult.Value);
            Assert.True(returnValue.Count() >= 0);
            
        }

    }
}
