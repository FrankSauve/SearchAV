using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace RC_SpeechToText.Models
{
	public class File
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string FilePath { get; set; }
		public DateTime? DateAdded { get; set; }
		public string Type { get; set; }
		public string Flag { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; }
		public Guid ReviewerId { get; set; }
		public User Reviewer { get; set; }
		public string Description { get; set; }
		public string ThumbnailPath { get; set; }
		public string Duration { get; set; }

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
				switch (value)
				{
					case FileFlag.Automatise:
						this.Flag = "Automatisé";
						break;
					case FileFlag.Edite:
						this.Flag = "Edité";
						break;
					case FileFlag.Revise:
						this.Flag = "Révisé";
						break;
					default:
						throw new NullReferenceException();
				}

				Console.WriteLine(this.FileFlag);
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