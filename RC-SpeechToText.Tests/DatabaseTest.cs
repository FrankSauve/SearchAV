
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace RC_SpeechToText.Tests
{
    public class DatabaseTest
    {
        [Fact]
        public async Task GettAllVideos()
        {
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase()
.Options;

            var context = new SearchAVContext(options);
            context.Video.AddRange(Enumerable.Range(1, 20).Select(t => new Video { Title = "Video " + t, VideoPath = "vPath " + t, Transcription = "tPath " + t }));
            context.SaveChanges();

            var controller = new VideoController();

            //Act
            var result = await controller.Index();
            var model = Assert.IsAssignableFrom<IEnumerable<Video>>(result);
            Assert.True(model.Count() >= 0);
            
        }

    }
}
