using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Models;
using System;
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

		public async Task DeleteWordsByFileId(Guid id)
		{
			var wordList = await _context.Version
				.Where(v => Guid.Equals(v.FileId, id))
				.Include(x => x.Words)
				.Select(x => x.Words)
				.FirstAsync();

			_context.Word.RemoveRange(wordList);
			await _context.SaveChangesAsync();
		}
	}
}
