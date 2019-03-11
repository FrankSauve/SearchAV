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
                var files = await _context.File.ToListAsync();
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


                var usernames = new List<string>();

                foreach (var file in files)
                {
                    var user = await _context.User.FindAsync(file.UserId);
                    usernames.Add(user.Name);
                }

                return Ok(Json(new { files, usernames }));
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


                var usernames = new List<string>();

                foreach (var file in files)
                {
                    var user = await _context.User.FindAsync(file.UserId);
                    usernames.Add(user.Name);
                }

                return Ok(Json(new { files, usernames }));
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


                var usernames = new List<string>();

                foreach (var file in files)
                {
                    var user = await _context.User.FindAsync(file.UserId);
                    usernames.Add(user.Name);
                }

                return Ok(Json(new { files, usernames }));
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

                var usernames = new List<string>();

                foreach (var file in files)
                {
                    _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t FILE TITLE: : " + file.Title);
                    var user = await _context.User.FindAsync(file.UserId);
                    usernames.Add(user.Name);
                }

                return Ok(Json(new { files, usernames }));
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
                var files = await _context.File.Where(f => f.ReviewerId == id && f.Flag != "Révisé").ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t USER FILES TO REVIEW: " + files.Count);

                var usernames = new List<string>();

                foreach (var file in files)
                {
                    _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t FILE TITLE: : " + file.Title);
                    var user = await _context.User.FindAsync(file.UserId);
                    usernames.Add(user.Name);
                }

                return Ok(Json(new { files, usernames }));
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
                return Ok(await _context.File.FindAsync(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching file with id: " + id);
                return BadRequest("File with ID" + id + " not found");
            }
        }

        [HttpGet("[action]/{search}")]
        public async Task<IActionResult> GetFilesByDescription(string search)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all files");
                var files = await _context.File.ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Files size: " + files.Count);

                search = search.Trim();

				var filesContainDescription = new List<File>();

				foreach (var file in files)
				{
					if (file.Description != null)
					{
						if (file.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							filesContainDescription.Add(file);

                            //If file is added here we do not want to add it again if it has a title match too
                            continue;
						}
					}
                    if (file.Title != null)
                    {
                        if (file.Title.Equals(search, StringComparison.InvariantCultureIgnoreCase))
                        {
                            filesContainDescription.Add(file);
                        }
                    }
                }

               

				return Ok(filesContainDescription);
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
			if (newTitle != null)
			{
                if (await VerifyIfTitleExists(newTitle))
                {
                    return BadRequest("Le nom de fichier existe déjà. Veuillez choisir un nouveau nom.");
                }
                else
                {
                    File file = _context.File.Find(id);
                    if (file.ThumbnailPath != "NULL")
                    {
                        file.ThumbnailPath = ModifyThumbnailName(file.Title, newTitle);
                    }
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

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> SaveDescription(int id, string newDescription)
        {
            File file = _context.File.Find(id);
            file.Description = newDescription;

            try
            {
                _context.File.Update(file);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated description for file with id: " + file.Id);
                return Ok(file);
            }
            catch
            {
                _logger.LogError("Error updating description for file with id: " + file.Id);
                return BadRequest("Description not updated");
            }

        }

        //Quick fix for now, does not work without it
        //TO DO: find a way to remove this
        [AllowAnonymous]  
        [HttpPost("[action]/{fileId}/{reviewerId}")]
        public async Task<IActionResult> AddReviewer(int fileId, int reviewerId)
        {
            _logger.LogInformation("AddReviewer for file with id: " + fileId);
            _logger.LogInformation("reviewerId: " + reviewerId);

            File file = _context.File.Find(fileId);
            file.ReviewerId = reviewerId;

            try
            {
                _context.File.Update(file);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated reviewerId for file with id: " + file.Id);
                _logger.LogInformation("File reviewerId: " + file.ReviewerId);
                return Ok(file);
            }
            catch
            {
                _logger.LogError("Error updating reviewerId for file with id: " + file.Id + " and reviewerId: " + reviewerId);
                return BadRequest("File reviewerId not updated");
            }

        }

        public async Task<bool> VerifyIfTitleExists(string title)
        {
            var files = await _context.File.ToListAsync();
            List<string> titleList = new List<string>();
            
            foreach(var file in files)
            {
                titleList.Add(file.Title.Trim());
            }

            if (titleList.Contains(title.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;      
        }

        private string ModifyThumbnailName(string oldName, string newName)
        {
            //Verifies if file exists in the current directory
            if (System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + @"\wwwroot\assets\Thumbnails\" + oldName + ".jpg"))
            {
                string oldPath = System.IO.Directory.GetCurrentDirectory() + @"\wwwroot\assets\Thumbnails\" + oldName + ".jpg";
                string newPath = System.IO.Directory.GetCurrentDirectory() + @"\wwwroot\assets\Thumbnails\" + newName + ".jpg";
                //Rename file in current directory to new title
                System.IO.File.Move(oldPath, newPath);
                return @"\assets\Thumbnails\" + newName + ".jpg";
            }
            else
                return "NULL";
        }
    }
}
