using System;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Version = RC_SpeechToText.Models.Version;
using System.Globalization;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SaveEditedTranscriptController : Controller
    {

        private readonly SearchAVContext _context;
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public SaveEditedTranscriptController(SearchAVContext context, ILogger<SaveEditedTranscriptController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveEditedTranscript(string versionId, string oldTranscript, string newTranscript)
        {
            _logger.LogInformation("versionId: " + versionId);
            _logger.LogInformation("New transcript: " + newTranscript);
            _logger.LogInformation("Old transcript: " + oldTranscript);

            //Converting versionId to an integer in oder to find the corresponding version
            int id = int.Parse(versionId);

            Version currentVersion = _context.Version.Find(id);

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
                file.Flag = "Edité";
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