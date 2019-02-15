using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class WordController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public WordController(SearchAVContext context, ILogger<FileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all words with versionId
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{versionId}")]
        public async Task<IActionResult> GetByVersionId(int versionId)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Getting all words for version with id: " + versionId);
                var words = await _context.Word.Where(w => w.VersionId == versionId).ToListAsync();
                return Ok(words);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Error getting all words for version with id: " + versionId);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Delete all words with fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpDelete("[action]/{fileId}")]
        public async Task<IActionResult> DeleteByFileId(int fileId)
        {

            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Deleting all words for file with id: " + fileId);

                var versions = await _context.Version.Where(v => v.FileId == fileId).ToListAsync();
                var wordList = new List<Word>();
                foreach(var version in versions)
                {
                    var words = await _context.Word.Where(w => w.VersionId == version.Id).ToListAsync();
                    wordList.AddRange(words);
                }

                _context.Word.RemoveRange(wordList);
                await _context.SaveChangesAsync();
                return Ok("Delete words from file with id: " + fileId);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Error deleting all words for file with id: " + fileId);
                return BadRequest(ex);
            }
        }
    }
}