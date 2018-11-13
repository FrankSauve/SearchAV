using System;
using System.Collections.Generic;

namespace RC_SpeechToText.Models
{
    public partial class Videos
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string VideoPath { get; set; }
        public string TranscriptionPath { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
