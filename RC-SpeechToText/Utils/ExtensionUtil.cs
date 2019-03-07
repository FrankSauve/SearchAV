using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Utils
{
	public static class ExtentionUtil
	{
		public static bool IsNullOrEmpty(this string str)
		{
			if (str == null || str.Trim() == "" || str.Length == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}