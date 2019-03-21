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
			File.Delete(path);
		}

		public void GenerateSRTFile(List<string> paragraph, List<string> timestamps, string fileTitle)
		{
			TextWriter tw = new StreamWriter(@"C:\Users\Philippe\Source\Repos\SearchAV\RC-SpeechToText\wwwroot\assets\Audio\" + fileTitle + ".srt");

			//Write each line as follow:
			//1 (the paragraph count)
			//00:00:00,000 --> 00:00:00,000
			//Paragraph
			//-blank line-
			var paragraphCount = 0;
			foreach (string p in paragraph)
			{
				tw.WriteLine(paragraphCount.ToString());
				tw.WriteLine(timestamps[paragraphCount] + " --> " + timestamps[paragraphCount + 1]);
				tw.WriteLine(paragraph[paragraphCount]);
				tw.WriteLine("");
				paragraphCount++;
			}
			tw.Close();
		}
	}
}
