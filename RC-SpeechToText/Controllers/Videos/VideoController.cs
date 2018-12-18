using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System.Linq;

namespace RC_SpeechToText.Controllers
{
    [Route("api/[controller]")]
    public class VideoController : Controller
    {
        SearchAVContext db = new SearchAVContext();

        // GET: api/<controller>
        [HttpGet("[action]")]
        public IActionResult Index()
        {
            try
            {
                return Ok(db.Video.ToList());
            }
            catch
            {
                return BadRequest("Get all videos failed.");
            }
        }

        [HttpPost("[action]")]
        public IActionResult Create(Video video)
        {
            try
            {
                db.Video.Add(video);
                db.SaveChanges();
                return Ok(video);
            }
            catch
            {
                return BadRequest("Video not created.");
            }
        }

        [HttpGet("[action]/{id}")]
        public IActionResult Details(int id)
        {
            try
            {
                return Ok(db.Video.Find(id));
            }
            catch
            {
                return BadRequest("Video with ID" + id + " not found");
            }
        }

        [HttpDelete("[action]/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Video video = db.Video.Find(id);
                db.Video.Remove(video);
                db.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest("Video with ID" + id + " not found");
            }
            
        }
    }
}
