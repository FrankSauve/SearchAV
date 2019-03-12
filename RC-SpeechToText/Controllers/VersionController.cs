using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
		private readonly VersionService _versionService; 
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public VersionController(SearchAVContext context, ILogger<FileController> logger)
        {
			_versionService = new VersionService(context);
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
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Fetching all versions");
                return Ok(await _versionService.Index());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Error fetching all versions");
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
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Fetching versions with fileId: " + fileId);
                return Ok(await _versionService.GetVersionByFileId(fileId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Error fetching versions with fileId: " + fileId);
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
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching active version with fileId: " + fileId);
                return Ok(await _versionService.GetFileActiveVersion(fileId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching active version with fileId: " + fileId);
                return BadRequest("Error fetching active version with fileId: " + fileId);
            }
        }

        [HttpDelete("[action]/{fileId}")]
        public async Task<IActionResult> DeleteFileVersions(int fileId)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Deleting all versions for file with id: " + fileId);
				await _versionService.DeleteFileVersions(fileId);

				return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Error all versions for file with id: "+ fileId);
                return BadRequest(ex);
            }
        }
    }
}
