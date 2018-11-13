using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RC_SpeechToText.Controllers
{
    [Route("api/[controller]")]
public class VideoController : Controller
{
        VideoDataAccessLayer video = new VideoDataAccessLayer();

        // GET: api/<controller>
        [HttpGet]
        [Route("api/Video/Index")]
        public IEnumerable<Videos> Index()
        {
            return video.GetAllVideos();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<controller>
    [HttpPost]
    public void Post([FromBody]string value)
    {
    }

    // PUT api/<controller>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/<controller>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
}
