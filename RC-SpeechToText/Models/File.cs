using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace RC_SpeechToText.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public DateTime? DateAdded { get; set; }
        public string Type { get; set; }
        public string Flag { get; set; }
		public int UserId { get; set; }
        public User User { get; set; }
        public int ReviewerId { get; set; }
        public User Reviewer { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }

		[NotMapped]
		public FileFlag FileFlag
		{
			get
			{
				switch (Flag)
				{
					case "Automatisé":
						return FileFlag.Automatise;
					case "Edité":
						return FileFlag.Edite;
					case "Révisé":
						return FileFlag.Revise;
					default:
						throw new NullReferenceException();
				}
			}

			set
			{
				FileFlag = value;
				switch (value)
				{
					case FileFlag.Automatise:
						Flag = "Automatisé";
						break;
					case FileFlag.Edite:
						Flag = "Edité";
						break;
					case FileFlag.Revise:
						Flag = "Révisé";
						break;
					default:
						throw new NullReferenceException();
				}
			}
		}
	}

    public enum FileFlag
    {
        Automatise,
        Edite,
        Revise
    }
}
