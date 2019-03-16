using System;
using System.Collections.Generic;

namespace RC_SpeechToText.Models
{
	public class Version
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid FileId { get; set; }
		public string Transcription { get; set; }
		public string HistoryTitle { get; set; }
		public bool Active { get; set; }
		public DateTime? DateModified { get; set; }

		public List<Word> Words { get; set; }
    }
}
