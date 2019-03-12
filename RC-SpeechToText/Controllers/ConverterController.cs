using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace RC_SpeechToText.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	public class ConverterController : Controller
	{
		private readonly SearchAVContext _context;
		private readonly ILogger _logger;

		public ConverterController(SearchAVContext context, ILogger<ConverterController> logger)
		{
			_context = context;
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
				ConvertionService convertionService = new ConvertionService(_context);
				var version = await convertionService.ConvertAndTranscribe(audioFile, userEmail);
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
