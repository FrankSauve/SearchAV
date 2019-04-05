using RC_SpeechToText.Infrastructure;
using RC_SpeechToText.Models;
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
        private readonly SearchAVContext _context;
        private IOInfrastructure streamIO = new IOInfrastructure();

        public ExportTranscriptionService(SearchAVContext context)
        {
            _context = context;
        }


		public async Task<bool> ExportVideo(string fileTitle, string documentType, string transcription, List<Models.Word> words)
		{
			var splitFileTitle = fileTitle.Split(".");
			var videoPath = streamIO.GetPathFromDirectory(@"\wwwroot\assets\Audio\");
			var subtitlePath = GetFfmpegSubtitlePath();
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
					"\"" +
					videoPath + 
					splitFileTitle[0] +
					".mp4\"" +
					" -vf subtitles=\'" + 
					"\"" +
					subtitlePath +
					splitFileTitle[0] +
					".srt\'" + 
					"\"" +
					" -max_muxing_queue_size 1024 " +
					"\"" +
					videoPath +
					splitFileTitle[0] +
					"Burn.mp4" +
					"\"";
			}
			else
			{
				if (streamIO.FileExist(videoPath + splitFileTitle[0] + "Embedded.mp4"))
					streamIO.DeleteFile(videoPath + splitFileTitle[0] + "Embedded.mp4");

				command =
					"-i " +
					"\"" +
					videoPath +
					splitFileTitle[0] +
					".mp4\" -i " +
					"\"" +
					videoPath +
					splitFileTitle[0] +
					".srt\" -c copy -c:s mov_text " +
					"\"" +
					videoPath + splitFileTitle[0] +
					"Embedded.mp4" +
					"\"";
			}

			var videoProcess = new ProcessStartInfo
			{
				CreateNoWindow = false,
				UseShellExecute = false,
				FileName = streamIO.CombinePath(streamIO.GetPathFromDirectory(@"\ffmpeg\bin"), "ffmpeg.exe"),
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
			var para = new List<string>();
			//get each paragraph. Remove all empty string (where <br> are present). Trim the strings
			var clearedTranscription = transcription.ClearHTMLTag();
			var paragraph = clearedTranscription.Split("\n");

			foreach (var p in paragraph)
				para.AddRange(p.SplitByCharCount(30));

			var timestamps = new List<string>();
			//Count all the word that have been already passed through. => O(logN^2)
			var wordPassed = 0;

			foreach(string p in para)
			{
				var paragraphWords = p.Split(" ").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
				timestamps.AddRange(GetParagraphTimestamp(paragraphWords, words.Skip(wordPassed).ToList()));
				wordPassed += paragraphWords.Count() - 1;
			}

			var splitFileTitle = fileTitle.Split(".");
			streamIO.GenerateSRTFile(para, timestamps, splitFileTitle[0]);

			return true;
		}

        /// <summary>
        /// Gets the file bytes of the specified file to be downloaded.
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="file"></param>
        /// <returns name="fileBytes"></returns>
        public byte[] GetFileBytes(string docType, Models.File file)
        {
            var net = new System.Net.WebClient();
            var splitFileTitle = file.Title.Split(".")[0];
            var videoPath = streamIO.GetPathFromDirectory(@"\wwwroot\assets\Audio\");
            byte[] fileBytes = null;

            switch (docType)
            {
                case "srt":
                    fileBytes = System.IO.File.ReadAllBytes(videoPath + splitFileTitle + ".srt");
                    break;
                case "video":
                    fileBytes = System.IO.File.ReadAllBytes(videoPath + splitFileTitle + "Embedded.mp4");
                    break;
                case "videoburn":
                    fileBytes = System.IO.File.ReadAllBytes(videoPath + splitFileTitle + "Burn.mp4");
                    break;
            }

            return fileBytes;
        }

        private List<string> GetParagraphTimestamp(List<string> paragraph, List<Word> words)
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
			var temp = string.Join(string.Empty, Regex.Matches(timestamp, @"\d+\.*\d*").OfType<Match>().Select(m => m.Value)).Split(".");

			var time = TimeSpan.FromSeconds(int.Parse(temp[0]));
			string timeStr = time.ToString(@"hh\:mm\:ss");

			if (temp.Length > 1)
				timeStr += "," + temp[1];
			else
				timeStr += ",000";

			return timeStr;
		}

		private string GetFfmpegSubtitlePath()
		{
			var subtitlePath = streamIO.GetPathFromDirectory("\\wwwroot\\assets\\Audio\\");
			var subtitlePathForwardSlash = subtitlePath.Replace("\\", "/");
			var colonIndex = subtitlePathForwardSlash.IndexOf(":");
			return subtitlePathForwardSlash.Insert(colonIndex, "\\");
		}
	}
}
