namespace RC_SpeechToText.Models.Google
{
    public class GoogleUser
    {
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        public string Email;
    }
}
