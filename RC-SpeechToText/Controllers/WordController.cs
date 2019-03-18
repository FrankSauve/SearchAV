using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Services;
using RC_SpeechToText.Filters;

namespace RC_SpeechToText.Controllers
{
    [ServiceFilter(typeof(ControllerExceptionFilter))]
    [ServiceFilter(typeof(LoggingActionFilter))]
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
                return Ok(await _wordService.GetByVersionId(versionId));
            }
            catch(Exception ex)
            {
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
            await _wordService.DeleteWordsByFileId(fileId);
            return Ok("Delete words from file with id: " + fileId);
        }
    }
}