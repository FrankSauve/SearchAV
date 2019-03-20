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

namespace RC_SpeechToText.Controllers
{
    [ServiceFilter(typeof(ControllerExceptionFilter))]
    [ServiceFilter(typeof(LoggingActionFilter))]
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

            var files = await _fileService.GetAllFiles();

            return Ok(files);

        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllWithUsernames()
        {
            var filesUsernames = await _fileService.GetAllWithUsernames();

            return Ok(filesUsernames);
        }

        [HttpGet("[action]/{flag}")]
        public async Task<IActionResult> GetAllFilesByFlag(string flag)
        {
            //Should find a better solution to handle accents
            var automated = Enum.GetName(typeof(FileFlag), 0);
            var edited = Enum.GetName(typeof(FileFlag), 1);
            var reviewed = Enum.GetName(typeof(FileFlag), 2);

            //If the flag is not accented we handle it here
            if (flag != automated && flag != edited && flag != reviewed)
                flag = (flag == "Automatise" ? automated : (flag == "Edite" ? edited : reviewed));

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

        [HttpGet("[action]/{date}")]
        public IActionResult FormatTime(string date)
        {
            return Ok(DateTimeUtil.FormatDateCardInfo(date));
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

        //Quick fix for now, does not work without it
        //TO DO: find a way to remove this
        [AllowAnonymous]
        [HttpPost("[action]/{fileId}/{userEmail}/{reviewerEmail}")]
        public async Task<IActionResult> AddReviewer(Guid fileId, string userEmail, string reviewerEmail)
        {
            var file = await _fileService.AddReviewer(fileId, userEmail, reviewerEmail);

            return Ok(file.File);
        }
    }
}
