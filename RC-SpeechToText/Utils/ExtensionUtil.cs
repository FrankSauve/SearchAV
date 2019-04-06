using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

		public static List<string> RemoveEmptyString(this List<string> str)
		{
			var newStringList = new List<string>();
			foreach(string s in str)
			{
				if(!s.IsNullOrEmpty())
				{
					newStringList.Add(s);
				}
			}

			return newStringList;
		}

		public static string RemovePunctuation(this string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;

			char[] anyOf = { '.', ',', ';', ':', '?', '!' };

			if (anyOf.Contains(str.Last()))
			{
				var newStr = str.Remove(str.Length - 1);
				return newStr;
			}
			else
				return str;
		}

		public static string ClearHTMLTag(this string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;

			var clearedTranscription = Regex.Replace(str, "<.*?>", string.Empty);
			return Regex.Replace(clearedTranscription, "&nbsp;", " ");
		}

		public static List<string> SplitByCharCount(this string text, int max)
		{
			var charCount = 0;
			var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			return lines.GroupBy(w => (charCount += w.Length + 1) / (max + 2))
						.Select(g => string.Join(" ", g.ToArray()))
						.ToList();
		}
	}
}