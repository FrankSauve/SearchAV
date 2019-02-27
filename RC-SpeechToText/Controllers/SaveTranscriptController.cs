using System;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Version = RC_SpeechToText.Models.Version;
using System.Globalization;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SaveTranscriptController : Controller
    {

        private readonly SearchAVContext _context;
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public SaveTranscriptController(SearchAVContext context, ILogger<SaveTranscriptController> logger)
        {
            _context = context;
            _logger = logger;
        }

		public async Task<IActionResult> DownloadTranscript(int documentType, string transcript)
		{ 
			var exportResult = await Task.Run(() => {
			   var exportTranscriptionService = new ExportTranscriptionService();
			   return exportTranscriptionService.CreateWordDocument(transcript);
			});

			_logger.LogInformation("Downloaded transcript: " + transcript);

			if (exportResult)
			{
				return Ok();
			}
			else
			{
				return BadRequest("Error while trying to download transcription");
			}
		}


		[HttpPost("[action]/{userId}/{versionId}")]
        public async Task<IActionResult> SaveTranscript(int userId, int versionId, string newTranscript)
        {
            _logger.LogInformation("versionId: " + versionId);
            Version currentVersion = _context.Version.Find(versionId);

            _logger.LogInformation("New transcript: " + newTranscript);
            _logger.LogInformation("Old transcript: " + currentVersion.Transcription);

            //Deactivate current version 
            _logger.LogInformation("current version active: " + currentVersion.Active);
            currentVersion.Active = false;

            //Update current version in DB
            try
            {
                _context.Version.Update(currentVersion);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated current version with id: " + currentVersion.Id);
            }
            catch
            {
                _logger.LogError("Error updating current version with id: " + currentVersion.Id);
            }

            //Create a new version
            Version newVersion = new Version
            {
                UserId = currentVersion.UserId,
                FileId = currentVersion.FileId,
                DateModified = DateTime.Now,
                Transcription = newTranscript,
                Active = true
            };

            //Add new version to DB
            try
            {
                await _context.Version.AddAsync(newVersion);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Added new version with id: " + newVersion.Id);
                _logger.LogInformation("New version transcript: " + newVersion.Transcription);
                _logger.LogInformation("New version fileId: " + newVersion.FileId);
            }
            catch
            {
                _logger.LogError("Error updating new version with id: " + newVersion.Id);
            }

            //Find corresponding file and update its flag 
            try
            {
                File file = await _context.File.FindAsync(newVersion.FileId);
                string flag = (file.ReviewerId == userId ? "Révisé" : "Edité"); //If user is reviewer of file, flag = "Révisé"
                _logger.LogInformation("FLAG: " + flag);
                file.Flag = flag;
                _context.File.Update(file);
                await _context.SaveChangesAsync();

                return Ok(newVersion);
            }
            catch
            {
                _logger.LogError("Error updating new version with id: " + newVersion.Id);
                return BadRequest("File flag not updated.");
            }
        }

    }
}