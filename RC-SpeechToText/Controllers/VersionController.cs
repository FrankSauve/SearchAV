using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Services;
using RC_SpeechToText.Filters;

namespace RC_SpeechToText.Controllers
{
    [ServiceFilter(typeof(ControllerExceptionFilter))]
    [ServiceFilter(typeof(LoggingActionFilter))]
    [Authorize]
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        private readonly VersionService _versionService;

        public VersionController(SearchAVContext context)
        {
            _versionService = new VersionService(context);
        }

        /// <summary>
        /// Returns all versions
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVersions()
        {

            return Ok(await _versionService.GetAllVersions());

        }

        /// <summary>
        /// Returns all versions with the fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{fileId}")]
        public async Task<IActionResult> GetbyFileId(Guid fileId)
        {
            return Ok(await _versionService.GetVersionByFileId(fileId));
        }


        /// <summary>
        /// Returns active version corresponding to the fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{fileId}")]
        public async Task<IActionResult> GetActivebyFileId(Guid fileId)
        {
            return Ok(await _versionService.GetFileActiveVersion(fileId));
        }

        /// <summary>
        /// Returns all versions with the corresponding user name corresponding to the fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{fileId}")]
        public async Task<IActionResult> GetAllVersionsWithUserName(Guid fileId)
        {

            var versionsUsernames = await _versionService.GetAllWithUsernames(fileId);

            return Ok(versionsUsernames);

        }

        [HttpDelete("[action]/{fileId}")]
        public async Task<IActionResult> DeleteFileVersions(Guid fileId)
        {
            await _versionService.DeleteFileVersions(fileId);
            return Ok();
        }
    }
}