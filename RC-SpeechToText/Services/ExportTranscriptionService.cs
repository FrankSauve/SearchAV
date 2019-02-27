using Word = Microsoft.Office.Interop.Word;

namespace RC_SpeechToText.Services
{
    public class ExportTranscriptionService
	{
		public void CreateWordDocument(string transcription)
		{
			Word.Application app = new Word.Application();
			Word.Document doc = app.Documents.Add();

			object start = 0;
			object end = transcription.Length;

			Word.Range range = doc.Range(ref start, ref end);
			range.Text = transcription;
			range.Select();

			doc.Save();
		}
	}
}
