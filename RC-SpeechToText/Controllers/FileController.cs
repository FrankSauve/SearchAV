using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
		private readonly FileService _fileService;

        public FileController(SearchAVContext context)
        {
			_fileService = new FileService(context);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            try
            {
				var files = await _fileService.GetAllFiles();

                return Ok(files);
            }
            catch
            {
                return BadRequest("Get all files failed.");
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllWithUsernames()
        {
            try
            {
				var filesUsernames = await _fileService.GetAllWithUsernames();

                return Ok(filesUsernames);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> getAllAutomatedFiles()
        {
            try
            {
				var filesUsernames = await _fileService.GetAllFilesByFlag("Automatisé");

                return Ok(filesUsernames);
            }
            catch
            {
                return BadRequest("Get all automated files failed.");
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> getAllEditedFiles()
        {
            try
            {
				var filesUsernames = await _fileService.GetAllFilesByFlag("Edité");

                return Ok(filesUsernames);
            }
            catch
            {
                return BadRequest("Get all edited files failed.");
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> getAllReviewedFiles()
        {
            try
            {
				var filesUsernames = await _fileService.GetAllFilesByFlag("Révisé");

                return Ok(filesUsernames);
            }
            catch
            {
                return BadRequest("Get all edited files failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> getAllFilesByUser(int id)
        {
            try
            {
				var filesUsernames = await _fileService.GetAllFilesById(id);

                return Ok(filesUsernames);
            }
            catch
            {
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
				var filesUsernames = await _fileService.GetUserFilesToReview(id);

                return Ok(filesUsernames);
            }
            catch
            {
                return BadRequest("Get user files to review failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
			{ 
                return Ok(await _fileService.GetFileById(id));
            }
            catch
            {
                return BadRequest("File with ID" + id + " not found");
            }
        }

        [HttpGet("[action]/{search}")]
        public async Task<IActionResult> GetFilesByDescriptionAndTitle(string search)
        {
            try
            {
				var files = await _fileService.GetAllFiles();
				return Ok(SearchService.SearchDescriptionAndTitle(files,search));
			}
            catch
            {
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
			var file = await _fileService.AddReviewer(fileId, reviewerId);

			if(file.Error != null)
			{
				return BadRequest(file.Error);
			}
			return Ok(file.File);
        }
    }
}
