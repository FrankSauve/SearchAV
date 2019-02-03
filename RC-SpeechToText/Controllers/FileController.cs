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

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public FileController(SearchAVContext context, ILogger<FileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all files");
                return Ok(await _context.File.ToListAsync());
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
                var files = await _context.File.ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Files size: " + files.Count);

                var usernames = new List<string>();

                foreach(var file in files)
                {
                    var user = await _context.User.FindAsync(file.UserId);
                    usernames.Add(user.Name);
                }

                return Ok(Json(new { files, usernames }));
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
                var files = await _context.File.Where(f => f.Flag == "Automatisé").ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t AUTOMATED FILES: " + files.Count);


                return Ok(files);
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
                var files = await _context.File.Where(f => f.Flag == "Edité").ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t EDITED FILES: " + files.Count);


                return Ok(files);
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
                var files = await _context.File.Where(f => f.Flag == "Révisé").ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t REVIEWED FILES: " + files.Count);


                return Ok(files);
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
                var files = await _context.File.Where(f => f.UserId == id).ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t USER FILES: " + files.Count);


                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching user files with userId -> " + id);
                return BadRequest("Get user files failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Fetching file with id: " + id);
                return Ok(await _context.File.FindAsync(id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Error fetching file with id: " + id);
                return BadRequest("File with ID" + id + " not found");
            }
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var file = new File { Id = id };
                _context.File.Attach(file);
                _context.File.Remove(file);
                await _context.SaveChangesAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Delete file with id: " + id);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - "+ this.GetType().Name +" \n\t Error deleting file with id: " + id);
                return BadRequest("File with ID" + id + " not found");
            }
        }
    }
}
