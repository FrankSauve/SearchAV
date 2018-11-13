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
        public int AddVideo(Videos video)
        {
            try
            {
                db.Videos.Add(video);
                db.SaveChanges();
                return 1;
            }
            catch
            {
                throw;
            }
        }

        public int AddVideo(Videos video, Videos path)
        {
            try
            {
                db.Videos.Add(video);
                db.Videos.Add(path);
                db.SaveChanges();
                return 1;
            }
            catch
            {
                throw;
            }
        }
        public int AddPath(Videos path)
        {
            try
            {
                db.Videos.Add(path);
                db.SaveChanges();
                return 1;
            }
            catch
            {
                throw;
            }
        }

        public Videos GetVideo(int id)
        {
            try
            {
                Videos video = db.Videos.Find(id);
                return video;
            }
            catch
            {
                throw;
            }
        }
    }
}
