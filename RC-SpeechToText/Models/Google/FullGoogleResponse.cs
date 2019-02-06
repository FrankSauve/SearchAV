namespace RC_SpeechToText.Models {
    public class FullGoogleResponse
    {
        public string Transcript { get; set; }
        public double Confidence { get; set; }
        public Words[] Words { get; set; }
    }

    public class Words
    {
        public string Word { get; set; }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }

    }

    public class Time
    {

        public double Seconds { get; set; }
        public double Nanos { get; set; }

    }
}
