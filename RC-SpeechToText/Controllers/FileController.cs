using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

		[HttpPut("[action]")]
		public async Task<IActionResult> ModifyTitle(string fileId, string newTitle)
		{
			int id = int.Parse(fileId);
			if (newTitle != null)
			{
				File file = _context.File.Find(id);

				file.Title = newTitle;

				try
				{
					_context.File.Update(file);
					await _context.SaveChangesAsync();
					_logger.LogInformation("Updated current title for file with id: " + file.Id);
					return Ok(file);
				}
				catch
				{
					_logger.LogError("Error updating current title for file with id: " + file.Id);
					return BadRequest("File title not updated");
				}
			}
			else
			{
				return BadRequest("Title is null");
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
