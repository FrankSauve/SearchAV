using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger _logger;
		private readonly UserService _userService;
        private readonly CultureInfo _dateConfig = new CultureInfo("en-GB");

        public UserController(SearchAVContext context, ILogger<UserController> logger)
        {
			_userService = new UserService(context);
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
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t User created: ID: "+user.Id+" Email: " + user.Email + " Name: " + user.Name);
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t User already exists, logging in as: ID:"+user.Id+" Email:" + user.Email + " Name:" + user.Name);
                return Ok(await _userService.CreateUser(user));
            }
            catch(Exception ex)
            {
                _logger.LogError(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n" + ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching all users");

                return Ok(await _userService.GetAllUsers());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching all users");
                return BadRequest("Get all users failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetUserName(int id)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching user with id: " + id);
                return Ok(await _userService.GetUserName(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching user with id: " + id);
                return BadRequest("User with ID" + id + " not found");
            }
        }

        [HttpGet("[action]/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Fetching user with email: " + email);

                return Ok(await _userService.GetUserByEmail(email));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n\t Error fetching user with email: " + email);
                return BadRequest("User with EMAIL '" + email + "' not found");
            }
        }
    }
}