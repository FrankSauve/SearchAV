using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models
{
    public static class DocumentType
	{
		public static string Doc
		{
			get
			{
				return "doc";
			}
		}

		public static string GoogleDoc
		{
			get
			{
				return "googleDoc";
			}
		}

		public static string Srt
		{
			get
			{
				return "srt";
			}
		}

		public static string Video
		{
			get
			{
				return "video";
			}
		}
	}
}
