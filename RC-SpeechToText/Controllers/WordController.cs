using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class WordController : Controller
    {
		private readonly WordService _wordService;

        public WordController(SearchAVContext context)
        {
			_wordService = new WordService(context);
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
        public async Task<IActionResult> DeleteWordsByFileId(int fileId)
        {
            try
            {
				await _wordService.DeleteWordsByFileId(fileId);
                return Ok("Delete words from file with id: " + fileId);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}