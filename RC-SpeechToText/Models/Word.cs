using System;

namespace RC_SpeechToText.Models
{
    public class Word
    {
        public Guid Id { get; set; }
        public string Term { get; set; }
        public string Timestamp { get; set; }
        public int VersionId { get; set; }
    }
}
