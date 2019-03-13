using System;
using System.Collections.Generic;

namespace RC_SpeechToText.Models
{
    public  class File
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public DateTime? DateAdded { get; set; }
        public string Type { get; set; }
        public string Flag { get; set; }
        public int UserId { get; set; }
        public int ReviewerId { get; set; }
        public User Reviewer { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }
    }
}
