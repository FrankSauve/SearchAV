using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using RC_SpeechToText.Filters;
using RC_SpeechToText.Exceptions;
using Microsoft.Extensions.Options;
using RC_SpeechToText.Models.DTO.Incoming;

namespace RC_SpeechToText.Controllers
{
	[ServiceFilter(typeof(ControllerExceptionFilter))]
	[ServiceFilter(typeof(LoggingActionFilter))]
	[Authorize]
	[Route("api/[controller]")]
	public class ConverterController : Controller
	{
		private readonly ConvertionService _convertionService;
		private readonly FileService _fileService;
		private AppSettings _appSettings { get; set; }

		public ConverterController(SearchAVContext context, IOptions<AppSettings> settings)
		{
			_appSettings = settings.Value;
			_fileService = new FileService(context, _appSettings);
			_convertionService = new ConvertionService(context, _appSettings);
		}

		/// <summary>
		/// Generates an automatic transcript using google cloud.
		/// GET: /api/googletest/speechtotext
		/// </summary>
		/// <returns>GoogleResult</returns>
		[HttpPost("[action]")]
		public async Task<IActionResult> ConvertAndTranscribe(IFormFile audioFile, string userEmail, string description, string title)
		{

			var version = await _convertionService.ConvertAndTranscribe(new ConvertionDTO(audioFile, userEmail, description, title));
			if (version == null)
			{
				throw new ControllerExceptions("Une erreur s'est produite lors de la transcription");
			}

			return Ok(version);
		}

	}
}
