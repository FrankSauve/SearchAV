using Google.Cloud.Speech.V1;
    public class GoogleResult
    {
        public LongRunningRecognizeResponse GoogleResponse { get; set; }
        public string ManualTranscript { get; set; }
        public double Accuracy { get; set; }
		public string Error { get; set; }
    }