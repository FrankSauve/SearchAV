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
        private readonly SearchAVContext _context;

        public VideoController(SearchAVContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IActionResult Index()
        {
            try
            {
                return Ok(_context.Video.ToList());
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
                _context.Video.Add(video);
                _context.SaveChanges();
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
                return Ok(_context.Video.Find(id));
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
                Video video = _context.Video.Find(id);
                _context.Video.Remove(video);
                _context.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest("Video with ID" + id + " not found");
            }
        }
    }
}
