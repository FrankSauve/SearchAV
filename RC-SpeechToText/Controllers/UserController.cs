using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

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
                // Store in DB
                await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n User created: " + user);

                return Ok(user);
            }
            catch(Exception ex)
            {
                _logger.LogError(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n" + ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> getUserName(int id)
        {
            try
            {
                _logger.LogInformation(DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Fetching user with id: " + id);
                return Ok(await _context.User.FindAsync(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString(_dateConfig) + " - " + this.GetType().Name + " \n Error fetching user with id: " + id);
                return BadRequest("User with ID" + id + " not found");
            }
        }
    }
}