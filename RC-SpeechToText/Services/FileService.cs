using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Exceptions;
using RC_SpeechToText.Infrastructure;
using RC_SpeechToText.Models;
using RC_SpeechToText.Models.DTO.Incoming;
using RC_SpeechToText.Models.DTO.Outgoing;
using RC_SpeechToText.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            return FormatTitles(await _context.File.ToListAsync());
        }

        public async Task<File> GetFileById(Guid id)
        {
            return FormatTitle(await _context.File.FindAsync(id));
        }

        public async Task<FileUsernameDTO> GetAllWithUsernames()
        {
            var files = await _context.File.Include(q => q.User).ToListAsync();
            files = FormatTitles(files);
            return new FileUsernameDTO { Files = files, Usernames = files.Select(x => x.User.Name).ToList() };
        }

        public async Task<FileUsernameDTO> GetAllFilesByFlag(string flag)
        {
			FileFlag fileFlag;
			switch (flag)
			{
				case "Automatise":
					fileFlag = FileFlag.Automatise;
					break;
				case "Edite":
					fileFlag = FileFlag.Edite;
					break;
				case "Revise":
					fileFlag = FileFlag.Revise;
					break;
				default:
					throw new NullReferenceException();
			}

			var files = await _context.File.Where(f => f.FileFlag == fileFlag).Include(q => q.User).ToListAsync();
            files = FormatTitles(files);
            return new FileUsernameDTO { Files = files, Usernames = files.Select(x => x.User.Name).ToList() };
        }

        public async Task<FileUsernameDTO> GetAllFilesById(string email)
        {
            var files = await _context.File.Where(f => f.User.Email == email).Include(q => q.User).ToListAsync();
            files = FormatTitles(files);
            return new FileUsernameDTO { Files = files, Usernames = files.Select(x => x.User.Name).ToList() };
        }

        public async Task<FileUsernameDTO> GetUserFilesToReview(string email)
        {
            var files = await _context.File.Where(f => f.Reviewer.Email == email && f.FileFlag != FileFlag.Revise).Include(q => q.User).ToListAsync();
            files = FormatTitles(files);
            return new FileUsernameDTO { Files = files, Usernames = files.Select(x => x.User.Name).ToList() };
        }

        public async Task<FileDTO> ModifyTitle(Guid id, string newTitle)
        {
            var proper = System.IO.Path.GetFileNameWithoutExtension(newTitle);
            if (proper != null)
            {
                if (await VerifyIfTitleExists(proper))
                {
                    throw new ControllerExceptions("Le nom de fichier existe déjà. Veuillez choisir un nouveau nom.");
                }
                else
                {
                    var file = _context.File.Find(id);
                    var ext = System.IO.Path.GetExtension(file.FilePath);
                    var oldTitle = System.IO.Path.GetFileNameWithoutExtension(file.Title);
                    

                    if (file.ThumbnailPath != "NULL")
                    {
                        file.ThumbnailPath = ModifyThumbnailName(oldTitle, proper);
                        file.FilePath = ModifyFileName(file.Title, proper, file.FilePath);
                    }
                    file.Title = proper + ext;


                    await _context.SaveChangesAsync();
                    return new FileDTO { File = file, Error = null };

                }
            }
            else
            {
                throw new ControllerExceptions("Title is null");
            }
        }

        public async Task DeleteFile(Guid id)
        {

            var file = new File { Id = id };
            _context.File.Attach(file);
            _context.File.Remove(file);
            await _context.SaveChangesAsync();
            RemoveFiles(id);
        }

        public async Task<FileDTO> SaveDescription(Guid id, string newDescription)
        {
            try
            {
                var file = _context.File.Find(id);
                file.Description = newDescription;

                await _context.SaveChangesAsync();
                return new FileDTO { File = file, Error = null };
            }
            catch
            {
                throw new ControllerExceptions("Description not updated");
            }
        }

		public async Task<FileDTO> AddReviewer(Guid fileId, string userEmail, string reviewerEmail)
		{

            var file = await _context.File.Where(f => f.Id == fileId).FirstOrDefaultAsync();
            var reviewer = await _context.User.Where(u => u.Email == reviewerEmail).FirstOrDefaultAsync();

			if (reviewer != null)
			{
				file.ReviewerId = reviewer.Id;
                await _context.SaveChangesAsync();

                //Send email to reviewer
                var askingUser = await _context.User.Where(u => u.Email == userEmail).FirstOrDefaultAsync();
                var emailSerice = new EmailInfrastructure();
                emailSerice.SendReviewAskedEmail(reviewer.Email, file, askingUser.Name);

                return new FileDTO { File = file, Error = null };
            }
			else
			{
                throw new ControllerExceptions("User not found");
			}
		}

		public async Task ModifyThumbnail(OutModifyThumbnailDTO modifyThumbnailDTO)
		{
			var file = await _context.File.Where(f => f.Id == modifyThumbnailDTO.FileId).FirstOrDefaultAsync();

			await Task.Run(() =>
			{
				var streamIO = new IOInfrastructure();
				
				var filePath = streamIO.GetPathFromDirectory(@"\wwwroot\assets\Audio\" + file.Title);
				var thumbnailPath = streamIO.GetPathFromDirectory(@"\wwwroot\assets\Thumbnails\" + file.Title.Split(".")[0] + ".jpg");

				streamIO.DeleteFile(thumbnailPath);

				var converter = new Converter();

				string thumbnailImage;
				if(modifyThumbnailDTO.SeekTime == TimeSpan.Parse(file.Duration).TotalMilliseconds)
					thumbnailImage = converter.CreateThumbnail(filePath, thumbnailPath, modifyThumbnailDTO.SeekTime-500);
				else
					thumbnailImage = converter.CreateThumbnail(filePath, thumbnailPath, modifyThumbnailDTO.SeekTime);

				if (thumbnailImage != null)
					Console.Write(thumbnailImage);
			});
		}

        public async Task<bool> VerifyIfTitleExists(string title)
        {
            var existingFileTitlesCount = await _context.File.CountAsync(x => EF.Functions.Like(x.Title, title.Trim() + ".%") || EF.Functions.Like(x.Title, title.Trim()));
            if (existingFileTitlesCount > 0)
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
                streamIO.MoveFilePath(oldPath, newPath);
                return @"\assets\Thumbnails\" + newName + ".jpg";
            }
            else
                return "NULL";
        }

        public string ModifyFileName(string oldName, string newName, string filePath)
        {   
            string ext = System.IO.Path.GetExtension(filePath);
            var streamIO = new IOInfrastructure();
            //Verifies if file exists in the current directory
            if (streamIO.VerifyPathExistInDirectory(@"\wwwroot\assets\Audio\" + oldName))
            {
                string oldPath = streamIO.GetPathFromDirectory(@"\wwwroot\assets\Audio\" + oldName);
                string newPath = streamIO.GetPathFromDirectory(@"\wwwroot\assets\Audio\" + newName + ext);
                //Rename file in current directory to new title
                streamIO.MoveFilePath(oldPath, newPath);
                return @"\assets\Audio\" + newName + ext;
            }
            else
                return "NULL";
        }

        public List<File> FormatTitles(List<File> files)
        {
            int count = 0;
            foreach (File f in files)
            {
                count++;
                f.Title = System.IO.Path.GetFileNameWithoutExtension(f.Title);
            };
            return files;
        }

        public File FormatTitle(File file)
        {
            file.Title = System.IO.Path.GetFileNameWithoutExtension(file.Title);
            return file;
        }

        public void RemoveFiles(Guid id)
        {
            var streamIO = new IOInfrastructure();
            var file = new File();
            file = _context.File.Find(id);
            var filePath = streamIO.GetPathFromDirectory(@"\wwwroot" + file.FilePath);
            var thumbnailPath = streamIO.GetPathFromDirectory(@"\wwwroot" + file.ThumbnailPath);
            streamIO.DeleteFile(filePath);
            streamIO.DeleteFile(thumbnailPath);
        }
    }
}
