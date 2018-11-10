using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RC_SpeechToText.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class TranscriptSearchController : Controller
    {
        [HttpPost("[action]")]
        public string SearchTranscript(string searchTerms)
        {

            

            return "hehe";

        }
    }
}