using System;
using System.Collections.Generic;

namespace RC_SpeechToText.Models
{
	public class Version
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int FileId { get; set; }
		public string Transcription { get; set; }
		public string HistoryTitle { get; set; }
		public bool Active { get; set; }
		public DateTime? DateModified { get; set; }

		public List<Word> Words { get; set; }
    }
}
