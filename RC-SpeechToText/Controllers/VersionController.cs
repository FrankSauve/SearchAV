using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;

        public VersionController(SearchAVContext context, ILogger<FileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Returns all versions
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Fetching all versions");
                return Ok(await _context.Version.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all versions");
                return BadRequest("Get all versions failed.");
            }
        }

        /// <summary>
        /// Returns all versions with the transcriptionId
        /// </summary>
        /// <param name="transcriptionId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{transcriptionId}")]
        public async Task<IActionResult> GetByTranscriptionId(int transcriptionId)
        {
            try
            {
                _logger.LogInformation("Fetching version with ID: " + transcriptionId);
                var versions = await _context.Version.Where(v => v.TranscriptionId == transcriptionId).FirstOrDefaultAsync();
                return Ok(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching version with ID: " + transcriptionId);
                return BadRequest("Error fetching version with ID: " + transcriptionId);
            }
        }
    }
}
