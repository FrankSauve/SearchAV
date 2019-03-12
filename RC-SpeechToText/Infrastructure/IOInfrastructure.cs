using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

		public void GenerateSRTFile(List<string> paragraph, List<string> timestamps)
		{
			//TODO: Find a way to prompt the user on the file path
			TextWriter tw = new StreamWriter(paragraph[0].Substring(0, 10) + ".srt");

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
