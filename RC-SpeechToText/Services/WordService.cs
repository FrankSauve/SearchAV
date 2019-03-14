using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    public class WordService
	{
		private readonly SearchAVContext _context;

		public WordService(SearchAVContext context)
		{
			_context = context;
		}

		public async Task DeleteWordsByFileId(int id)
		{
			var wordList = await _context.Version
				.Where(v => v.FileId == id)
				.Include(x => x.Words)
				.Select(x => x.Words)
				.FirstAsync();

			_context.Word.RemoveRange(wordList);
			await _context.SaveChangesAsync();
		}

        public async Task<List<Word>> GetByVersionId(int versionId)
        {
            var words = await _context.Word.Where(w => w.VersionId == versionId).ToListAsync();
            return words;
        }
	}
}
