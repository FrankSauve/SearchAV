using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models
{
    public class AppSettings
	{
		public string AudioPath { get; set; }
		public string FfmpegPath { get; set; }
		public string ThumbnailPath { get; set; }
		public string Root { get; set; }
	}
}
