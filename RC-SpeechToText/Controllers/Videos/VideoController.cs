using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace RC_SpeechToText.Controllers
{
    [Route("api/[controller]")]
    public class VideoController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;

        public VideoController(SearchAVContext context, ILogger<VideoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Fetching all videos");
                return Ok(_context.Video.ToList());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching all videos");
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
                _logger.LogInformation("Created video with id: " + video.VideoId);
                return Ok(video);
            }
            catch
            {
                _logger.LogError("Error creating video with id: " + video.VideoId);
                return BadRequest("Video not created.");
            }
        }

        [HttpGet("[action]/{id}")]
        public IActionResult Details(int id)
        {
            try
            {
                _logger.LogInformation("Fetching video with id: " + id);
                return Ok(_context.Video.Find(id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching video with id: " + id);
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
                _logger.LogInformation("Delete video with id: " + id);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting video with id: " + id);
                return BadRequest("Video with ID" + id + " not found");
            }
        }
    }
}
