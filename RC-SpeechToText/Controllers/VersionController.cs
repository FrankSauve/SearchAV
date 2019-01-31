using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
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
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

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
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Fetching all versions");
                return Ok(await _context.Version.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Error fetching all versions");
                return BadRequest("Get all versions failed.");
            }
        }

        /// <summary>
        /// Returns all versions with the transcriptionId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{fileId}")]
        public async Task<IActionResult> GetbyFileId(int fileId)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Fetching versions with fileId: " + fileId);
                var versions = await _context.Version.Where(v => v.FileId == fileId).ToListAsync();
                return Ok(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n Error fetching versions with fileId: " + fileId);
                return BadRequest("Error fetching versions with fileId: " + fileId);
            }
        }

        /// <summary>
        /// Returns active version corresponding to the fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{fileId}")]
        public async Task<IActionResult> GetActivebyFileId(int fileId)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Fetching active version with fileId: " + fileId);
                var version = await _context.Version.Where(v => v.FileId == fileId).Where(v => v.Active == true).FirstOrDefaultAsync();
                return Ok(version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Error fetching active version with fileId: " + fileId);
                return BadRequest("Error fetching active version with fileId: " + fileId);
            }
        }

        [HttpDelete("[action]/{fileId}")]
        public async Task<IActionResult> DeleteFileVersions(int fileId)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Deleting all versions for file with id: " + fileId);

                var versionsList = await _context.Version.Where(v => v.FileId == fileId).ToListAsync();

                foreach (var version in versionsList)
                {
                    _context.Version.Remove(version);
                    await _context.SaveChangesAsync();
                }

                return Ok("ok all versions are deleted for file with id: " + fileId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Error all versions for file with id: "+ fileId);
                return BadRequest("Delete all versions failed.");
            }
        }
    }
}
