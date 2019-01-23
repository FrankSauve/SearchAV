using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Authorization;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;

        public FileController(SearchAVContext context, ILogger<FileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Fetching all files");
                return Ok(_context.File.ToList());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching all files");
                return BadRequest("Get all files failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public IActionResult Details(int id)
        {
            try
            {
                _logger.LogInformation("Fetching file with id: " + id);
                return Ok(_context.File.Find(id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching file with id: " + id);
                return BadRequest("File with ID" + id + " not found");
            }
        }

        [HttpDelete("[action]/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var file = new File { FileId = id };
                _context.File.Attach(file);
                _context.SaveChanges();
                _logger.LogInformation("Delete file with id: " + id);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting file with id: " + id);
                return BadRequest("File with ID" + id + " not found");
            }
        }
    }
}
