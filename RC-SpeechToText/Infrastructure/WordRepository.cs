using RC_SpeechToText.Utils;
using Word = Microsoft.Office.Interop.Word;

namespace RC_SpeechToText.Infrastructure
{
    public class WordRepository
	{
		public bool CreateWordDocument(string transcription)
		{
			try
			{
				if (!transcription.IsNullOrEmpty())
				{
					var wordDocument = GenerateWordDocument(transcription);
					wordDocument.Save();

					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}

		private Word.Document GenerateWordDocument(string transcription)
		{
			Word.Application app = new Word.Application();
			Word.Document doc = app.Documents.Add();

			object start = 0;
			object end = 0;

			Word.Range range = doc.Range(ref start, ref end);
			range.Text = transcription;
			range.Select();

			return doc;
		}
	}
}
