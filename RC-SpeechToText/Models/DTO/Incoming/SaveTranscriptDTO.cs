using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models.DTO.Incoming
{
    public class SaveTranscriptDTO
	{
		private string emailString;

		public SaveTranscriptDTO(string emailString, Guid versionId, string newTranscript)
		{
			this.emailString = emailString;
			VersionId = versionId;
			NewTranscript = newTranscript;
		}

		public string UserEmail { get; set; }
		public Guid VersionId { get; set; }
		public string NewTranscript { get; set; }
	}
}
