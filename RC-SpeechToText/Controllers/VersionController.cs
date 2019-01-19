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
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Fetching all versions");
                return Ok(_context.Version.ToList());
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
        public IActionResult GetByTranscriptionId(int transcriptionId)
        {
            try
            {
                _logger.LogInformation("Fetching version with ID: " + transcriptionId);
                var versions = _context.Version.Where(v => v.TranscriptionId == transcriptionId);
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
