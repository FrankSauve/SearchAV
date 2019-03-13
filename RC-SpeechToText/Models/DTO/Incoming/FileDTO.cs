using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models.DTO.Incoming
{
    public class FileDTO
	{
		public File File { get; set; }
		public string Error { get; set; }
	}
}
