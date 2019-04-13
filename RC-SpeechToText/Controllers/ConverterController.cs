using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using RC_SpeechToText.Filters;
using RC_SpeechToText.Exceptions;
using System.Collections.Generic;
using System;

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

		public ConverterController(SearchAVContext context)
		{
			_convertionService = new ConvertionService(context);
            _fileService = new FileService(context);
		}

		/// <summary>
		/// Generates an automatic transcript using google cloud.
		/// GET: /api/googletest/speechtotext
		/// </summary>
		/// <returns>GoogleResult</returns>
		[HttpPost("[action]")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> ConvertAndTranscribe(List<IFormFile> files, string userEmail, string description, string title)
		{
           
                var count = await _convertionService.ProcessFiles(files, userEmail, description, title);
                if (count.Count > 0)
                {
                    throw new ControllerExceptions(count.Title);
                }
                return Ok();

            }
			
		}
}
