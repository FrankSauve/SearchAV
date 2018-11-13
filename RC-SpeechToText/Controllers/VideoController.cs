using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RC_SpeechToText.Controllers
{
[Route("api/[controller]")]
public class VideoController : Controller
{
VideoDataAccessLayer videoObj = new VideoDataAccessLayer();
        private SearchAVContext context;

        public VideoController(SearchAVContext context)
        {
            this.context = context;
        }

        // GET: api/<controller>
        [HttpGet]
[Route("api/Video/Index")]
public async Task<IEnumerable<Videos>> Index()
{
    return videoObj.GetAllVideos();
}

[HttpPost]
[Route("api/Video/Add")]
public int Create(Videos video)
{
    return videoObj.AddVideo(video);
}

[HttpPost]
[Route("api/Video/Add")]
public int Create(Videos video, Videos path)
{
    return videoObj.AddVideo(video, path);
}

[HttpPost]
[Route("api/Video/Add")]
public int CreatePath(Videos path)
{
    return videoObj.AddVideo(path);
}

[HttpGet]
[Route("api/Video/Details/{id}")]
public Videos Details(int id)
{
    return videoObj.GetVideo(id);
}

[HttpDelete]
[Route("api/Video/Delete/{id}")]
public int Delete(int id)
{
    return videoObj.RemoveVideo(id);
}
}
}
