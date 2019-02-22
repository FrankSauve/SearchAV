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
                var emails = new List<string>();

                foreach (var email in emailList)
                {
                    //emails.Add(email.Email);
                    MailMessage mail = new MailMessage("rcemail1819@gmail.com", email.Email);
                    SmtpClient client = new SmtpClient();
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("rcemail1819@gmail.com", "capstone1819");
                    client.UseDefaultCredentials = true;
                    client.Credentials = credentials;
                    client.Host = "smtp.gmail.com";
                    mail.Subject = "Testing Subject";
                    mail.Body = "Testing Body";
                    client.Send(mail);
                }
                return Ok(emailList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching editors");
                return BadRequest("Get all editors failed.");
            }
        }
    }
}