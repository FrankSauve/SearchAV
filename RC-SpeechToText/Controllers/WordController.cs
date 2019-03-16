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

        [HttpDelete("[action]/{fileId}")]
        public async Task<IActionResult> DeleteWordsByFileId(Guid fileId)
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