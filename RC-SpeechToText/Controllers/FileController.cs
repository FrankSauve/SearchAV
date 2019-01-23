using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Fetching all files");
                return Ok(await _context.File.ToListAsync());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching all files");
                return BadRequest("Get all files failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation("Fetching file with id: " + id);
                return Ok(await _context.File.FindAsync(id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching file with id: " + id);
                return BadRequest("File with ID" + id + " not found");
            }
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var file = new File { FileId = id };
                _context.File.Attach(file);
                _context.File.Remove(file);
                await _context.SaveChangesAsync();
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
