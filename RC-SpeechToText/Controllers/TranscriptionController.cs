using System;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Version = RC_SpeechToText.Models.Version;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TranscriptionController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public TranscriptionController(SearchAVContext context, ILogger<TranscriptionController> logger)
        {
            _context = context;
            _logger = logger;
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

            //Calling this method will handle saving the new words in the databse
           try
            {
                await SaveWords(versionId, newVersion.Id, newTranscript);
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Added words related to the new version: " + newVersion.Id + " to the database");
            }
            catch
            {
                _logger.LogError("Error saving new words with id: " + newVersion.Id);
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
                //Send email to user who uploaded file stating that review is done
                if (flag == "Révisé")
                {
                    var uploader = await _context.User.FindAsync(file.UserId);
                    var reviewer = await _context.User.FindAsync(file.ReviewerId);
                    var emailSerice = new EmailService();
                    emailSerice.SendReviewDoneEmail(uploader.Email, file, reviewer.Name);
                    _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Email sent to: " + uploader.Email + " with the file id: " + file.Id);

                }

                return Ok(newVersion);
            }
            catch
            {
                _logger.LogError("Error updating new version with id: " + newVersion.Id);
                return BadRequest("File flag not updated.");
            }
        }

        /// <summary>
        /// Private method that handles saving new words in the database when SaveTranscript is called
        /// This makes the transcript still searchable after adding new words
        /// </summary>
        private async Task<IActionResult> SaveWords(int versionId, int newVersionId, string newTranscript)
        {

            //Have to explicitly instantiate variable to be able to keep the words.
            List<Word> oldWords = new List<Word>();

            //Getting all the words for this versionId
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all words for versionId: " + versionId);
      
                //Ordered by Id to get the words in the same order as transcript
                oldWords = await _context.Word.Where(w => w.VersionId == versionId).OrderBy(w => w.Id).ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all words for versionId: " + versionId);
                return BadRequest("Error fetching words with versionId: " + versionId);
            }


            //Modify timestamps and return the new words
            var modifyTimeStampService = new ModifyTimeStampService();
            List<Word> newWords = modifyTimeStampService.ModifyTimestamps( oldWords, newTranscript, newVersionId );


            //Add all the words of the transcript to the database
            try
            {
                foreach ( var word in newWords)
                {
                    await _context.Word.AddAsync(word);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error adding a word to the database associated with new version: " + newVersionId);
                return BadRequest("Error adding words with versionId: " + newVersionId);
            }


            return Ok();
        }

        
        /// <summary>
        /// Returns all versions
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all versions");
                return Ok(await _context.Version.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all versions");
                return BadRequest("Get all versions failed.");
            }
        }

        /// <summary>
        /// Returns timestamps of searched terms
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        [HttpGet("[action]/{versionId}/{searchTerms}")]
        public async Task<IActionResult> SearchTranscript(string searchTerms, int versionId)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all words for versionId: " + versionId);

                //Ordered by Id to get the words in the same order as transcript
                var words = await _context.Word.Where(w => w.VersionId == versionId).OrderBy(w => w.Id).ToListAsync();
                var searchService = new SearchService(); 
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Searching for " + searchTerms);
                return Ok(searchService.PerformSearch(searchTerms, words));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all words for versionId: " + versionId);
                return BadRequest("Error fetching active version with fileId: " + versionId);
            }
        }


        [HttpGet("[action]/{fileId}/{documentType}")]
        public async Task<IActionResult> DownloadTranscript(string documentType, int fileId)
        {
            _logger.LogInformation(documentType);

            var version = _context.Version.Where(v => v.FileId == fileId).Where(v => v.Active == true).SingleOrDefault(); //Gets the active version (last version of transcription)
            var rawTranscript = version.Transcription;
            var transcript = rawTranscript.Replace("<br>", "\n");


            var exportResult = await Task.Run(() => {
                var exportTranscriptionService = new ExportTranscriptionService();
                if (documentType == "doc")
                {
                    return exportTranscriptionService.CreateWordDocument(transcript);
                }
                else if (documentType == "googleDoc")
                {
                    return exportTranscriptionService.CreateGoogleDocument(transcript);
                }
                else
                {
                    _logger.LogInformation("Invalid doc type");
                    return false;
                }
            });

            if (exportResult)
            {
                _logger.LogInformation("Downloaded transcript: " + transcript);
                return Ok();
            }
            else
            {
                return BadRequest("Error while trying to download transcription");
            }
        }

        
    }
}