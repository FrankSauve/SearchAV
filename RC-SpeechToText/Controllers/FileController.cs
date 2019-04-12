using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Utils;
using RC_SpeechToText.Services;
using System.Linq;
using RC_SpeechToText.Filters;
using RC_SpeechToText.Exceptions;
using RC_SpeechToText.Models.DTO.Outgoing;
using Microsoft.Extensions.Options;

namespace RC_SpeechToText.Controllers
{
    [ServiceFilter(typeof(ControllerExceptionFilter))]
    [ServiceFilter(typeof(LoggingActionFilter))]
    [Authorize]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly FileService _fileService;
		private AppSettings _appSettings { get; set; }

		public FileController(SearchAVContext context, IOptions<AppSettings> settings)
        {
			_appSettings = settings.Value;
			_fileService = new FileService(context, _appSettings);	
		}

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllFiles()
        {

            var files = await _fileService.GetAllFiles();

            return Ok(files);

        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllWithUsernames()
        {
            var filesUsernames = await _fileService.GetAllWithUsernames();

            return Ok(filesUsernames);
        }

        [HttpGet("{flag}")]
        public async Task<IActionResult> GetAllFilesByFlag(string flag)
        {
            var filesUsernames = await _fileService.GetAllFilesByFlag(flag);
            return Ok(filesUsernames);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllFilesByUser()
        {
            var emailClaim = HttpContext.User.Claims;
            var emailString = emailClaim.FirstOrDefault(c => c.Type == "email").Value;

            var filesUsernames = await _fileService.GetAllFilesById(emailString);

            return Ok(filesUsernames);

        }
        

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserFilesToReview()
        {

            var emailClaim = HttpContext.User.Claims;
            var emailString = emailClaim.FirstOrDefault(c => c.Type == "email").Value;

            var filesUsernames = await _fileService.GetUserFilesToReview(emailString);

            return Ok(filesUsernames);


        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {

            return Ok(await _fileService.GetFileById(id));

        }

        [HttpGet("[action]/{search}")]
        public async Task<IActionResult> GetFilesByDescriptionAndTitle(string search)
        {

            var files = await _fileService.GetAllFiles();
            return Ok(SearchService.SearchDescriptionAndTitle(files, search));


        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> ModifyTitle(Guid id, string newTitle)
        {
            var file = await _fileService.ModifyTitle(id, newTitle);

            return Ok(file.File);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _fileService.DeleteFile(id);

            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> SaveDescription(Guid id, string newDescription)
        {
            var file = await _fileService.SaveDescription(id, newDescription);

            return Ok(file.File);
        }
        
        [HttpGet("{fileId}/{reviewerEmail}")]
        public async Task<IActionResult> AddReviewer(Guid fileId, string reviewerEmail)
        {
            var emailClaim = HttpContext.User.Claims;
            var userEmail = emailClaim.FirstOrDefault(c => c.Type == "email").Value;

            var file = await _fileService.AddReviewer(fileId, userEmail, reviewerEmail);

            return Ok(file.File);
        }

        [HttpPost("[action]/{title}")]
        public async Task<IActionResult>VerifyIfTitleExists(string title)
        {
            var name = await _fileService.VerifyIfTitleExists(title);
            if (name == true)
            {
                throw new ControllerExceptions("Le nom de fichier existe déjà. Veuillez choisir un nouveau nom.");
            }

            return Ok(name);
        }
		[HttpGet("[action]/{fileId}/{seekTime}")]
		public async Task<IActionResult> ChangeThumbnail(Guid fileId, int seekTime)
		{
			var outDTO = new OutModifyThumbnailDTO { FileId = fileId, SeekTime = seekTime };
			await _fileService.ModifyThumbnail(outDTO);
			return Ok();
		}
    }
}
