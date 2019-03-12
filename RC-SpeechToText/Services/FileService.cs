using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Infrastructure;
using RC_SpeechToText.Models;
using RC_SpeechToText.Models.DTO.Incoming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    public class FileService
	{
		private readonly SearchAVContext _context;

		public FileService(SearchAVContext context)
		{
			_context = context;
		}

		public async Task<List<File>> GetAllFiles()
		{
			return await _context.File.ToListAsync();
		}

		public async Task<File> GetFileById(int id)
		{
			return await _context.File.FindAsync(id);
		}

		public async Task<FileUsernameDTO> GetAllWithUsernames()
		{
			var files = await _context.File.ToListAsync();

			var usernames = new List<string>();

			foreach (var file in files)
			{
				var user = await _context.User.FindAsync(file.UserId);
				usernames.Add(user.Name);
			}

			return new FileUsernameDTO { Files = files, Usernames = usernames};
		}

		public async Task<FileUsernameDTO> GetAllFilesByFlag(string flag)
		{
			var files = await _context.File.Where(f => f.Flag == flag).ToListAsync();

			var usernames = new List<string>();

			foreach (var file in files)
			{
				var user = await _context.User.FindAsync(file.UserId);
				usernames.Add(user.Name);
			}

			return new FileUsernameDTO { Files = files, Usernames = usernames };
		}

		public async Task<FileUsernameDTO> GetAllFilesById(int id)
		{
			var files = await _context.File.Where(f => f.UserId == id).ToListAsync();
			var usernames = new List<string>();

			foreach (var file in files)
			{
				var user = await _context.User.FindAsync(file.UserId);
				usernames.Add(user.Name);
			}

			return new FileUsernameDTO { Files = files, Usernames = usernames };
		}

		public async Task<FileUsernameDTO> GetUserFilesToReview(int id)
		{
			var files = await _context.File.Where(f => f.ReviewerId == id && f.Flag != "Révisé").ToListAsync();

			var usernames = new List<string>();

			foreach (var file in files)
			{
				var user = await _context.User.FindAsync(file.UserId);
				usernames.Add(user.Name);
			}

			return new FileUsernameDTO { Files = files, Usernames = usernames };
		}

		public async Task<FileDTO> ModifyTitle(int id, string newTitle)
		{
			if (newTitle != null)
			{
				if (await VerifyIfTitleExists(newTitle))
				{
					return new FileDTO { File = null, Error = "Le nom de fichier existe déjà. Veuillez choisir un nouveau nom." };
				}
				else
				{
					File file = _context.File.Find(id);

					if (file.ThumbnailPath != "NULL")
					{
						file.ThumbnailPath = ModifyThumbnailName(file.Title, newTitle);
					}
					file.Title = newTitle;

					try
					{
						_context.File.Update(file);
						await _context.SaveChangesAsync();
						return new FileDTO { File = file, Error = null };
					}
					catch
					{
						return new FileDTO { File = null, Error = "File title not updated" };
					}
				}
			}
			else
			{
				return new FileDTO { File = null, Error = "Title is null" };
			}
		}

		public async Task<string> DeleteFile(int id)
		{
			try
			{
				var file = new File { Id = id };
				_context.File.Attach(file);
				_context.File.Remove(file);
				await _context.SaveChangesAsync();
				return null;
			}
			catch
			{
				return "File with ID" + id + " not found";
			}
		}

		public async Task<FileDTO> SaveDescription(int id, string newDescription)
		{
			try
			{
				var file = _context.File.Find(id);
				file.Description = newDescription;

				_context.File.Update(file);
				await _context.SaveChangesAsync();
				return new FileDTO { File = file, Error = null };
			}
			catch
			{
				return new FileDTO { File = null, Error = "Description not updated" };
			}
		}

		public async Task<FileDTO> AddReviewer(int fileId, int reviewerId)
		{
			var file = _context.File.Find(fileId);
			file.ReviewerId = reviewerId;

			try
			{
				_context.File.Update(file);
				await _context.SaveChangesAsync();
				return new FileDTO { File = file, Error = null };
			}
			catch
			{
				return new FileDTO { File = null, Error = "File reviewerId not updated" };
			}
		}

		private async Task<bool> VerifyIfTitleExists(string title)
		{
			var files = await _context.File.ToListAsync();
			List<string> titleList = new List<string>();

			foreach (var file in files)
			{
				titleList.Add(file.Title.Trim());
			}

			if (titleList.Contains(title.Trim(), StringComparer.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private string ModifyThumbnailName(string oldName, string newName)
		{
			var streamIO = new IOInfrastructure();
			//Verifies if file exists in the current directory
			if (streamIO.VerifyPathExistInDirectory(@"\wwwroot\assets\Thumbnails\" + oldName + ".jpg"))
			{
				string oldPath = streamIO.GetPathFromDirectory(@"\wwwroot\assets\Thumbnails\" + oldName + ".jpg");
				string newPath = streamIO.GetPathFromDirectory(@"\wwwroot\assets\Thumbnails\" + newName + ".jpg");
				//Rename file in current directory to new title
				streamIO.MoveFilePath(oldName, newPath);
				return @"\assets\Thumbnails\" + newName + ".jpg";
			}
			else
				return "NULL";
		}
	}
}
