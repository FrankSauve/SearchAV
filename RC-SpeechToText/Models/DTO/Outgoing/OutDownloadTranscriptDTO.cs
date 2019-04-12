using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models.DTO.Outgoing
{
    public class OutDownloadTranscriptDTO
    {
		public Guid FileId { get; set; }
		public string DocumentType { get; set; }
	}
}
