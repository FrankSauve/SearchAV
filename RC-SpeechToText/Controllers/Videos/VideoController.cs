using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;

namespace RC_SpeechToText.Controllers
{
    [Route("api/[controller]")]
    public class VideoController : Controller
    {
        VideoDataAccessLayer videoObj = new VideoDataAccessLayer();

        // GET: api/<controller>
        [HttpGet("[action]")]
        public async Task<IEnumerable<Video>> Index()
        {
            return videoObj.GetAllVideos();
        }

        [HttpPost("[action]")]
        public int Create(Video video, Video path)
        {
            return videoObj.AddVideo(video, path);
        }

        [HttpPost("[action]")]
        public int CreatePath(Video path)
        {
            return videoObj.AddVideo(path);
        }

        [HttpGet("[action]/{id}")]
        public Video Details(int id)
        {
            return videoObj.GetVideo(id);
        }

        [HttpDelete("[action]/{id}")]
        public int Delete(int id)
        {
            return videoObj.RemoveVideo(id);
        }
    }
}
