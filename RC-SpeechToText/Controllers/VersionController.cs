﻿using System.Threading.Tasks;
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

        public VersionController(SearchAVContext context)
        {
			_versionService = new VersionService(context);
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
                return Ok(await _versionService.Index());
            }
            catch (Exception ex)
            {
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
                return Ok(await _versionService.GetVersionByFileId(fileId));
            }
            catch (Exception ex)
            {
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
                return Ok(await _versionService.GetFileActiveVersion(fileId));
            }
            catch (Exception ex)
            {
                return BadRequest("Error fetching active version with fileId: " + fileId);
            }
        }

        [HttpDelete("[action]/{fileId}")]
        public async Task<IActionResult> DeleteFileVersions(int fileId)
        {
            try
            {
                await _versionService.DeleteFileVersions(fileId);
				return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
