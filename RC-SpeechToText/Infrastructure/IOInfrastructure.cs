using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace RC_SpeechToText.Infrastructure
{
    public class IOInfrastructure
	{
		public string CopyAudioToStream(IFormFile audioFile, string path)
		{
			// Create the directory
			Directory.CreateDirectory(Directory.GetCurrentDirectory() + path);

			// Saves the file to the audio directory
			var filePath = Directory.GetCurrentDirectory() + path + audioFile.FileName;
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				audioFile.CopyTo(stream);
			}

			return filePath;
		}

		public string GetPathAndCreateDirectory(string path)
		{
			var completePath = Directory.GetCurrentDirectory() + path;
			Directory.CreateDirectory(completePath);
			return completePath;
		}

		public bool FileExist(string path)
		{
			return File.Exists(path);
		}

		public string GetPathFromDirectory(string path)
		{
			return Directory.GetCurrentDirectory() + path;
		}

		public bool VerifyPathExistInDirectory(string path)
		{
			return File.Exists(Directory.GetCurrentDirectory() + path);
		}

		public void MoveFilePath(string p1, string p2)
		{
			File.Move(p1, p2);
		}

		public string CombinePath(string p1, string p2)
		{
			return Path.Combine(p1, p2);
		}

		public void DeleteFile(string path)
		{
			if(File.Exists(path))
				File.Delete(path);
		}

        public void GenerateSRTFile(List<string> paragraph, List<string> timestamps, string fileTitle, Models.AppSettings appSettings)
		{
			TextWriter tw = new StreamWriter(GetPathFromDirectory(appSettings.AudioPath + fileTitle + ".srt"));

			//Write each line as follow:
			//1 (the paragraph count)
			//00:00:00,000 --> 00:00:00,000
			//Paragraph
			//-blank line-
			for(int i = 0, j = 0; i<paragraph.Count; i++, j = j+2)
			{
				tw.WriteLine(i.ToString());
				tw.WriteLine(timestamps[j] + " --> " + timestamps[j + 1]);
				tw.WriteLine(paragraph[i]);
				tw.WriteLine("");
			}
			tw.Close();
		}
	}
}
