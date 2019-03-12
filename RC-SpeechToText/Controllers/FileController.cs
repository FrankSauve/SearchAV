using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly ILogger _logger;
		private readonly FileService _fileService;
		private readonly SearchAVContext _context;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public FileController(SearchAVContext context, ILogger<FileController> logger)
        {
			_fileService = new FileService(context);
			_context = context;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all files");
				var files = await _fileService.Index();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t FILES FOUND: " + files.Count);

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all files");
                return BadRequest("Get all files failed.");
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllWithUsernames()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Fetching all files");
				var filesUsernames = await _fileService.GetAllWithUsernames();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Files size: " + filesUsernames.Files.Count);

                return Ok(filesUsernames);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Error fetching all files");
                return BadRequest(ex);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> getAllAutomatedFiles()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all automated files");
				var filesUsernames = await _fileService.GetAllFilesByFlag("Automatisé");
				_logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t AUTOMATED FILES: " + filesUsernames.Files.Count);

                return Ok(filesUsernames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all automated files");
                return BadRequest("Get all automated files failed.");
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> getAllEditedFiles()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all edited files");
				var filesUsernames = await _fileService.GetAllFilesByFlag("Edité");
				_logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t EDITED FILES: " + filesUsernames.Files.Count);

                return Ok(filesUsernames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all edited files");
                return BadRequest("Get all edited files failed.");
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> getAllReviewedFiles()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all reviewed files");
				var filesUsernames = await _fileService.GetAllFilesByFlag("Révisé");
				_logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t REVIEWED FILES: " + filesUsernames.Files.Count);

                return Ok(filesUsernames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all edited files");
                return BadRequest("Get all edited files failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> getAllFilesByUser(int id)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all files with userId: " + id);
				var filesUsernames = await _fileService.GetAllFilesById(id);
				_logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t USER FILES: " + filesUsernames.Files.Count);

                return Ok(filesUsernames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching user files with userId -> " + id);
                return BadRequest("Get user files failed.");
            }
        }

        [HttpGet("[action]/{date}")]
        public IActionResult FormatTime(string date)
        {
            return Ok(DateTimeUtil.FormatDateCardInfo(date));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> getUserFilesToReview(int id)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all files to review for user with  userId: " + id);
				var filesUsernames = await _fileService.GetUserFilesToReview(id);
				_logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t USER FILES TO REVIEW: " + filesUsernames.Files.Count);

                return Ok(filesUsernames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching user files to review with userId -> " + id);
                return BadRequest("Get user files to review failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
			{ 
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching file with id: " + id);
                return Ok(await _fileService.GetFileById(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching file with id: " + id);
                return BadRequest("File with ID" + id + " not found");
            }
        }

        [HttpGet("[action]/{search}")]
        public async Task<IActionResult> GetFilesByDescriptionAndTitle(string search)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all files");
                var files = await _context.File.ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Files size: " + files.Count);

				return Ok(SearchService.SearchDescriptionAndTitle(files,search));
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching files");
                return BadRequest("Error retrieving files");
            }
        }

        [HttpPut("[action]/{id}")]
		public async Task<IActionResult> ModifyTitle(int id, string newTitle)
		{
			var file = await _fileService.ModifyTitle(id, newTitle);

			if(file.Error != null)
			{
				return BadRequest(file.Error);
			}

			return Ok(file.File);
		}

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
			var error = await _fileService.DeleteFile(id);

			if (error != null)
			{
				return BadRequest(error);
			}

			return Ok();
		}

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> SaveDescription(int id, string newDescription)
        {
			var file = await _fileService.SaveDescription(id, newDescription);

			if (file.Error != null)
			{
				return BadRequest(file.Error);
			}

			return Ok(file.File);
		}

        //Quick fix for now, does not work without it
        //TO DO: find a way to remove this
        [AllowAnonymous]  
        [HttpPost("[action]/{fileId}/{reviewerId}")]
        public async Task<IActionResult> AddReviewer(int fileId, int reviewerId)
        {
            _logger.LogInformation("AddReviewer for file with id: " + fileId);
            _logger.LogInformation("reviewerId: " + reviewerId);

			var file = await _fileService.AddReviewer(fileId, reviewerId);

			if(file.Error != null)
			{
				_logger.LogError("Error updating reviewerId for file with id: " + file.File.Id + " and reviewerId: " + reviewerId);
				return BadRequest(file.Error);
			}

			_logger.LogInformation("Updated reviewerId for file with id: " + file.File.Id);
			_logger.LogInformation("File reviewerId: " + file.File.ReviewerId);
			return Ok(file.File);
        }
    }
}
