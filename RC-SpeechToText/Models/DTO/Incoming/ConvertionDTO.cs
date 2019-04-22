using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models.DTO.Incoming
{
    public class ConvertionDTO
	{
		public ConvertionDTO(IFormFile audioFile, string userEmail, string description, string title)
		{
			AudioFile = audioFile;
			UserEmail = userEmail;
			Description = description;
			Title = title;
		}

		public IFormFile AudioFile { get; set; }
		public string UserEmail { get; set; }
		public string Description { get; set; }
		public string Title { get; set; }
	}
}
