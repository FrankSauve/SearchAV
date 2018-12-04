using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models
{
    public class VideoDataAccessLayer
    {
        SearchAVContext db = new SearchAVContext();

        public IEnumerable<Video> GetAllVideos()
        {
            try
            {
                return db.Video.ToList();
            }
            catch
            {
                throw;
            }
        }
        public int AddVideo(Video video)
        {
            try
            {
                db.Video.Add(video);
                db.SaveChanges();
                return 1;
            }
            catch
            {
                throw;
            }
        }

        public int AddVideo(Video video, Video path)
        {
            try
            {
                db.Video.Add(video);
                db.Video.Add(path);
                db.SaveChanges();
                return 1;
            }
            catch
            {
                throw;
            }
        }
        public int AddPath(Video path)
        {
            try
            {
                db.Video.Add(path);
                db.SaveChanges();
                return 1;
            }
            catch
            {
                throw;
            }
        }

        public Video GetVideo(int id)
        {
            try
            {
                Video video = db.Video.Find(id);
                return video;
            }
            catch
            {
                throw;
            }
        }

        public int RemoveVideo(int id)
        {
            try
            {
                Video video = db.Video.Find(id);
                db.Video.Remove(video);
                db.SaveChanges();
                return 1;
            }
            catch
            {
                throw;
            }
        }
    }
}
