using System.Collections.Generic;

namespace RC_SpeechToText.Models.DTO.Incoming
{
    public class VersionUsernameDTO
    {
		public List<Version> Versions { get; set; }
		public List<string> Usernames { get; set; }
	}
}
