using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Models;
using System.Collections.Generic;
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
			var versions = await _context.Version.Where(v => v.FileId == id).ToListAsync();
			var wordList = new List<Word>();
			foreach (var version in versions)
			{
				var words = await _context.Word.Where(w => w.VersionId == version.Id).ToListAsync();
				wordList.AddRange(words);
			}

			_context.Word.RemoveRange(wordList);
			await _context.SaveChangesAsync();
		}
	}
}
