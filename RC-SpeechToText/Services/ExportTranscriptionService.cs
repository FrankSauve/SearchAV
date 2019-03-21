using RC_SpeechToText.Infrastructure;
using RC_SpeechToText.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    public class ExportTranscriptionService
	{
		private IOInfrastructure streamIO = new IOInfrastructure();

		public async Task<bool> ExportVideo(string fileTitle, string documentType, string transcription, List<Models.Word> words)
		{
			var splitFileTitle = fileTitle.Split(".");
			var videoPath = @"C:\Users\Philippe\Source\Repos\SearchAV\RC-SpeechToText\wwwroot\assets\Audio\";
			var subtitlePath = "C\\:/Users/Philippe/Source/Repos/SearchAV/RC-SpeechToText/wwwroot/assets/Audio/";
			string command;

			if(!streamIO.FileExist(videoPath + splitFileTitle[0] + ".srt"))
			{
				await Task.Run(() => CreateSRTDocument(transcription, words, fileTitle));
			}

			if (documentType.Contains("burn"))
			{
				if(streamIO.FileExist(videoPath + splitFileTitle[0] + "Burn.mp4"))
					streamIO.DeleteFile(videoPath + splitFileTitle[0] + "Burn.mp4");

				command =
					"-i " +
					videoPath + splitFileTitle[0] +
					".mp4 -vf subtitles=\'" +
					subtitlePath +
					splitFileTitle[0] +
					".srt\'" +
					" -max_muxing_queue_size 1024 " +
					videoPath +
					splitFileTitle[0] +
					"Burn.mp4";
			}
			else
			{
				if (streamIO.FileExist(videoPath + splitFileTitle[0] + "Embedded.mp4"))
					streamIO.DeleteFile(videoPath + splitFileTitle[0] + "Embedded.mp4");

				command =
					"-i " +
					videoPath +
					splitFileTitle[0] +
					".mp4 -i " +
					videoPath +
					splitFileTitle[0] +
					".srt -c copy -c:s mov_text " +
					videoPath + splitFileTitle[0] +
					"Embedded.mp4";
			}

			var videoProcess = new ProcessStartInfo
			{
				CreateNoWindow = false,
				UseShellExecute = false,
				FileName = streamIO.CombinePath(@"C:\Users\Philippe\Source\Repos\SearchAV\RC-SpeechToText\ffmpeg\bin", "ffmpeg.exe"),
				Arguments = command,
				RedirectStandardOutput = true
			};

			try
			{
				using (Process process = Process.Start(videoProcess))
				{
					while (!process.StandardOutput.EndOfStream)
					{
						string line = process.StandardOutput.ReadLine();
						Console.WriteLine(line);
					}

					process.WaitForExit();
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
		}


		public bool CreateSRTDocument(string transcription, List<Models.Word> words, string fileTitle)
		{
			//get each paragraph. Remove all empty string (where <br> are present). Trim the strings
			var paragraph = transcription.Split("\n").ToList().RemoveEmptyString().Select(str => str.Trim()).ToList();
			var timestamps = new List<string>();
			//Count all the word that have been already passed through. => O(logN^2)
			var wordPassed = 0;

			foreach(string p in paragraph)
			{
				var paragraphWords = p.Split(" ");
				timestamps.AddRange(GetParagraphTimestamp(paragraphWords, words.Skip(wordPassed).ToList()));
				wordPassed += paragraphWords.Count() - 1;
			}

			var splitFileTitle = fileTitle.Split(".");
			streamIO.GenerateSRTFile(paragraph, timestamps, splitFileTitle[0]);

			return true;
		}

		private List<string> GetParagraphTimestamp(string[] paragraph, List<Models.Word> words)
		{
			//Look for the first instance where the paragraph word match & the word db match
			var firstWord = words.Find(x => x.Term == paragraph.First());
			//Same but for the last word
			var lastWord = words.Find(x => x.Term == paragraph.Last().RemovePunctuation());
			return new List<string>
			{
				//Save the time stamp
				FormatTimestamp(firstWord.Timestamp),
				FormatTimestamp(lastWord.Timestamp)
			};
		}

		private string FormatTimestamp(string timestamp)
		{
			//getting this "\"4.600s\"", should be this 00:00:04,600
			var temp = string.Join(string.Empty, Regex.Matches(timestamp, @"\d+").OfType<Match>().Select(m => m.Value));

			for (int i = temp.Count(); i < 9; i++) //there is always 9 numbers
			{
				temp = "0" + temp;
			}

			temp = temp.Insert(2, ":").Insert(5, ":").Insert(8, ",");
			return temp;
		}
	}
}
