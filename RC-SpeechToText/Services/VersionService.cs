using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Models;
using RC_SpeechToText.Models.DTO.Incoming;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    public class VersionService
	{
		private readonly SearchAVContext _context;

		public VersionService(SearchAVContext context)
		{
			_context = context;
		}

		public async Task<List<Version>> Index()
		{
			return await _context.Version.ToListAsync();
		}

		public async Task<List<Version>> GetVersionByFileId(int id)
		{
			return await _context.Version.Where(v => v.FileId == id).ToListAsync();
		}

		public async Task<Version> GetFileActiveVersion(int id)
		{
			return await _context.Version.Where(v => v.FileId == id).Where(v => v.Active == true).FirstOrDefaultAsync();
		}

        public async Task<VersionUsernameDTO> GetAllWithUsernames(int id)
        {
            var versions = await _context.Version.Where(v => v.FileId == id).ToListAsync();

            var usernames = new List<string>();

            foreach (var version in versions)
            {
                var user = await _context.User.FindAsync(version.UserId);
                usernames.Add(user.Name);
            }

            return new VersionUsernameDTO { Versions = versions, Usernames = usernames };
        }

        public async Task DeleteFileVersions(int id)
		{
			var versionsList = await _context.Version.Where(v => v.FileId == id).ToListAsync();
			_context.Version.RemoveRange(versionsList);
			await _context.SaveChangesAsync();
		}
	}
}
