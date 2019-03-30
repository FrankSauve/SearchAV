using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models.DTO.Outgoing
{
    public class OutModifyThumbnailDTO
	{
		public Guid FileId { get; set; }
		public int SeekTime { get; set; }
	}
}
