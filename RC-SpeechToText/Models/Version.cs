namespace RC_SpeechToText.Models
{
    public class Version
    {
        public int VersionId { get; set; }
        public int UserId { get; set; }
        public int TranscriptionId { get; set; }
        public string Transcription { get; set; }
        public int Active { get; set; }
    }
}
