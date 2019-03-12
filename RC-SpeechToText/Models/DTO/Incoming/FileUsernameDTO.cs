using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models.DTO.Incoming
{
    public class FileUsernameDTO
	{
		public List<File> Files { get; set; }

		public List<string> Usernames { get; set; }
	}
}
