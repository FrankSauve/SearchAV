using System;
using System.Collections.Generic;

namespace RC_SpeechToText.Models
{
    public  class Video
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string VideoPath { get; set; }
        public string Transcription { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
