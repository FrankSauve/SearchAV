using Google.Cloud.Speech.V1;

namespace RC_SpeechToText.Services
{
    public class TranscriptionService
    {
		/// <summary>
		/// Transcribe the input file to text using Google Cloud
		/// </summary>
		/// <param name="inputFilePath"></param>
		/// <returns name="googleResult"></returns>
		public static GoogleResult GoogleSpeechToText(string inputFilePath)
		{
			try
			{
				var speech = SpeechClient.Create();
				var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
				{
					Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
					LanguageCode = "fr-ca",
					EnableWordTimeOffsets = true // Required to get timestamps
				}, RecognitionAudio.FromFile(inputFilePath));

				longOperation = longOperation.PollUntilCompleted();
				var response = longOperation.Result;

				var googleResult = new GoogleResult
				{
					GoogleResponse = response.Results[0]
				};

				return googleResult;
			}
			catch (System.Exception e)
			{
				var googleResult = new GoogleResult
				{
					Error = e.Message
				};

				return googleResult;
			}
		}
    }
}
