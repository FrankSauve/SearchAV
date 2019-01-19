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

        [HttpPost("[action]")]
        public IActionResult Create(File file)
        {
            try
            {
                _context.File.Add(file);
                _context.SaveChanges();
                _logger.LogInformation("Created file with id: " + file.FileId);
                return Ok(file);
            }
            catch
            {
                _logger.LogError("Error creating file with id: " + file.FileId);
                return BadRequest("File not created.");
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
                File file = _context.File.Find(id);
                _context.File.Remove(file);
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
