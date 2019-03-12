using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace RC_SpeechToText.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	public class ConverterController : Controller
	{
		private readonly ILogger _logger;
		private readonly ConvertionService _convertionService;

		public ConverterController(SearchAVContext context, ILogger<ConverterController> logger)
		{
			_convertionService = new ConvertionService(context);
			_logger = logger;
		}

		/// <summary>
		/// Generates an automatic transcript using google cloud.
		/// GET: /api/googletest/speechtotext
		/// </summary>
		/// <returns>GoogleResult</returns>
		[HttpPost("[action]")]
		public async Task<IActionResult> ConvertAndTranscribe(IFormFile audioFile, string userEmail)
		{
			try
			{
				var version = await _convertionService.ConvertAndTranscribe(audioFile, userEmail);
				if(version == null)
				{
					return BadRequest("Une erreur s'est produite lors de la transcription");
				}

				return Ok(version);
			}
			catch
			{
				return BadRequest("Une erreur s'est produite lors de la transcription");
			}
		}
	}
}
