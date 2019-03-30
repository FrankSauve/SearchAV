using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;
using Microsoft.AspNetCore.Authorization;
using RC_SpeechToText.Services;
using RC_SpeechToText.Filters;
using System;

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

            return Ok(await _userService.CreateUser(user));


        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUsers()
        {

            return Ok(await _userService.GetAllUsers());


        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetUserName(Guid id)
        {

            return Ok(await _userService.GetUserName(id));

        }

        [HttpGet("[action]/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {

            return Ok(await _userService.GetUserByEmail(email));

        }
    }
}