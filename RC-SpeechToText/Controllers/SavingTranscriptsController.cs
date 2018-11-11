using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace RC_SpeechToText.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class SavingTranscriptsController : Controller
    {
        [HttpPost("[action]")]
        public void SaveTranscript(string JSON)
        {
            string path = System.IO.Path.GetFullPath(@"..\transcriptions\test.json");
            // This text is added only once to the file.
            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to.

                System.IO.File.WriteAllText(path, JSON);
            }

           

            // Open the file to read from.
            //string readText = File.ReadAllText(path);
            //Console.WriteLine(readText);




        }
    }
}