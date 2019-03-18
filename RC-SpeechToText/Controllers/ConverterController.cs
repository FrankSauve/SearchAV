using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using RC_SpeechToText.Filters;
using RC_SpeechToText.Exceptions;

namespace RC_SpeechToText.Controllers
{
    [ServiceFilter(typeof(ControllerExceptionFilter))]
    [ServiceFilter(typeof(LoggingActionFilter))]
    [Authorize]
	[Route("api/[controller]")]
	public class ConverterController : Controller
	{
		private readonly ConvertionService _convertionService;

		public ConverterController(SearchAVContext context)
		{
			_convertionService = new ConvertionService(context);
		}

		/// <summary>
		/// Generates an automatic transcript using google cloud.
		/// GET: /api/googletest/speechtotext
		/// </summary>
		/// <returns>GoogleResult</returns>
		[HttpPost("[action]")]
		public async Task<IActionResult> ConvertAndTranscribe(IFormFile audioFile, string userEmail)
		{
			
				var version = await _convertionService.ConvertAndTranscribe(audioFile, userEmail);
				if(version == null)
				{
                    throw new ControllerExceptions("Une erreur s'est produite lors de la transcription"); 
				}

				return Ok(version);
			
		}
	}
}
