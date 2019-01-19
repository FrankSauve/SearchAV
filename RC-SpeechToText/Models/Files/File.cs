using System;
using System.Collections.Generic;

namespace RC_SpeechToText.Models
{
    public  class File
    {
        public int FileId { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public int TranscriptionId { get; set; }
        public DateTime? DateAdded { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
    }
}
