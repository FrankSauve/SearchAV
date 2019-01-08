using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Primitives;
using RC_SpeechToText.Models;
using System.Net.Http;
using Newtonsoft.Json;
using RC_SpeechToText.Models.Google;

namespace RC_SpeechToText.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public UserController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string googleToken = "";

            if (Request.Headers.TryGetValue("Authorization", out StringValues headerValues))
            {
                googleToken = headerValues.FirstOrDefault().ToString().Substring(7);
            }

            var request = new HttpRequestMessage(HttpMethod.Get,
            "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + googleToken);

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            dynamic googleUser = JsonConvert.DeserializeObject(response.Content.ToString());

            return Ok(googleUser.Payload);
            
        }
    }
}