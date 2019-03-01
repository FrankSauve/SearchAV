using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace RC_SpeechToText.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly SearchAVContext _context;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public UserController(SearchAVContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Create new user and store in DB
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody]User user)
        {
            try
            {
                if (!await _context.User.AnyAsync(u => u.Email == user.Email))
                {
                    // Store in DB
                    await _context.User.AddAsync(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t User created: ID: "+user.Id+" Email: " + user.Email + " Name: " + user.Name);

                    return Ok(user);
                }
                
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t User already exists, logging in as: ID:"+user.Id+" Email:" + user.Email + " Name:" + user.Name);
                return Ok(user);
                
                
            }
            catch(Exception ex)
            {
                _logger.LogError(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n" + ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> getAllUsers()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all users");
                var users = await _context.User.ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t USERS FOUND: " + users.Count);

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all users");
                return BadRequest("Get all users failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> getUserName(int id)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching user with id: " + id);
                return Ok(await _context.User.FindAsync(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching user with id: " + id);
                return BadRequest("User with ID" + id + " not found");
            }
        }

        [HttpGet("[action]/{email}")]
        public async Task<IActionResult> getUserByEmail(string email)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching user with email: " + email);
                var user = await _context.User.Where(u => u.Email == email).FirstOrDefaultAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t User Found! USER ID: " + user.Id);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching user with email: " + email);
                return BadRequest("User with EMAIL '" + email + "' not found");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> sendMail()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching names and emails for all editors");
                var emailList = await _context.User.Where(u => u.Job_Tile == "Editor").ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t EDITORS FOUND: " + emailList.Count);

                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all files with notification set to 1");
                var fileList = await _context.File.Where(f => f.Notified == 1).ToListAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t FILES FOUND: " + fileList.Count);

                var body = new StringBuilder();

                foreach (var email in emailList)
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("rcemail1819@gmail.com");
                    SmtpClient smtp = new SmtpClient();
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("rcemail1819@gmail.com", "capstone1819");
                    smtp.Host = "smtp.gmail.com";
                    mail.To.Add(new MailAddress(email.Email));
                    mail.IsBodyHtml = true;
                    mail.Subject = "Nouvelles Transcriptions";
                    foreach (var file in fileList)
                    {    
                        body.AppendLine("<a href='http://localhost:59723/FileView/" + file.Id + "'>"+ file.Title + "</a><br />");
                        //file.Notified = 0;
                    }
                    mail.Body = "Liste de transcription: " + "<br />" + body.ToString();
                    smtp.Send(mail);
                    smtp.Dispose();
                    _context.SaveChanges();


                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching editors");
                return BadRequest("Get all editors failed.");
            }
        }
    }
}