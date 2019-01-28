using System;

namespace RC_SpeechToText.Models
{
    public class Version
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FileId { get; set; }
        public string Transcription { get; set; }
        public Boolean Active { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
