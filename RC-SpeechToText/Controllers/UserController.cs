using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Services;
using RC_SpeechToText.Filters;

namespace RC_SpeechToText.Controllers
{
    [ServiceFilter(typeof(ControllerExceptionFilter))]
    [ServiceFilter(typeof(LoggingActionFilter))]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
		private readonly UserService _userService;

        public UserController(SearchAVContext context)
        {
			_userService = new UserService(context);
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
                return Ok(await _userService.CreateUser(user));
            }
            catch
            {
                return BadRequest("Create user failed");
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                return Ok(await _userService.GetAllUsers());
            }
            catch
            {
                return BadRequest("Get all users failed.");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetUserName(int id)
        {
            try
            {
                return Ok(await _userService.GetUserName(id));
            }
            catch
            {
                return BadRequest("User with ID" + id + " not found");
            }
        }

        [HttpGet("[action]/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                return Ok(await _userService.GetUserByEmail(email));
            }
            catch
            {
                return BadRequest("User with EMAIL '" + email + "' not found");
            }
        }
    }
}