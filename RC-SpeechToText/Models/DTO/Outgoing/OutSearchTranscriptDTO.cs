using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models.DTO.Outgoing
{
    public class OutSearchTranscriptDTO
    {
		public Guid VersionId { get; set; }
		public string SearchTerms { get; set; }
	}
}
