using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models
{
    public class VideoDataAccessLayer
{
        SearchAVContext db = new SearchAVContext();

        public IEnumerable<Videos> GetAllVideos()
        {
            try
            {
                return db.Videos.ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
