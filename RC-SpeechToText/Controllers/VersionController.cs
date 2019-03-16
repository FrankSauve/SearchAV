﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System;
using Microsoft.AspNetCore.Authorization;
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
            catch
            {
                return BadRequest("Get all versions failed.");
            }
        }

        /// <summary>
        /// Returns all versions with the fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{fileId}")]
        public async Task<IActionResult> GetbyFileId(Guid fileId)
        {
            try
            {
                return Ok(await _versionService.GetVersionByFileId(fileId));
            }
            catch
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
        public async Task<IActionResult> GetActivebyFileId(Guid fileId)
        {
            try
            {
                return Ok(await _versionService.GetFileActiveVersion(fileId));
            }
            catch
            {
                return BadRequest("Error fetching active version with fileId: " + fileId);
            }
        }

        /// <summary>
        /// Returns all versions with the corresponding user name corresponding to the fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{fileId}")]
        public async Task<IActionResult> GetAllVersionsWithUserName(Guid fileId)
        {
            try
            {
                var versionsUsernames = await _versionService.GetAllWithUsernames(fileId);

                return Ok(versionsUsernames);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("[action]/{fileId}")]
        public async Task<IActionResult> DeleteFileVersions(Guid fileId)
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