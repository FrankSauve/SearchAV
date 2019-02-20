using Google.Cloud.Speech.V1;
    public class GoogleResult
    {
        public SpeechRecognitionResult GoogleResponse { get; set; }
        public string ManualTranscript { get; set; }
        public double Accuracy { get; set; }
    }