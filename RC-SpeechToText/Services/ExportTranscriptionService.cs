using Word = Microsoft.Office.Interop.Word;

namespace RC_SpeechToText.Services
{
    public class ExportTranscriptionService
	{
		public bool CreateWordDocument(string transcription)
		{
			try
			{
				Word.Application app = new Word.Application();
				Word.Document doc = app.Documents.Add();

				object start = 0;
				object end = 0;

				Word.Range range = doc.Range(ref start, ref end);
				range.Text = transcription;
				range.Select();

				doc.Save();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
