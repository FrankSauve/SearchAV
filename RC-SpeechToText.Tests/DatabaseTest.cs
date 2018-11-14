
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
            context.Videos.AddRange(Enumerable.Range(1, 20).Select(t => new Videos { Title = "Video " + t, VideoPath = "vPath " + t, TranscriptionPath = "tPath " + t }));
            context.SaveChanges();

            var controller = new VideoController(context);

            //Act
            var result = await controller.Index();
            var model = Assert.IsAssignableFrom<IEnumerable<Videos>>(result);
            Assert.Equal(3, model.Count());
            
        }

    }
}
