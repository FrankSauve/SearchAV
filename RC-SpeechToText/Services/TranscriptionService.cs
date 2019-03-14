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
    public class TranscriptionService
	{
		private readonly SearchAVContext _context;

		public TranscriptionService(SearchAVContext context)
		{
			_context = context;
		}

		public async Task<List<Models.Version>> Index()
		{
			return await _context.Version.ToListAsync();
		}

		public async Task<VersionDTO> SaveTranscript(string userEmail, int versionId, string newTranscript)
		{
			var currentVersion = _context.Version.Find(versionId);

			//Deactivate current version 
			currentVersion.Active = false;

			//Update current version in DB
			try
			{
				_context.Version.Update(currentVersion);
				await _context.SaveChangesAsync();
			}
			catch
			{
				return new VersionDTO { Version = null, Error = "Error updating current version with id: " + currentVersion.Id };
			}

			//Create a new version
			var newVersion = new Models.Version
			{
				UserId = currentVersion.UserId,
				FileId = currentVersion.FileId,
				DateModified = DateTime.Now,
				Transcription = newTranscript,
				Active = true
			};

			//Add new version to DB
			try
			{
				await _context.Version.AddAsync(newVersion);
				await _context.SaveChangesAsync();
			}
			catch
			{
				return new VersionDTO { Version = null, Error = "Error updating new version with id: " + newVersion.Id };
			}

			//Calling this method will handle saving the new words in the databse
			try
			{
				var resultSaveWords = await SaveWords(versionId, newVersion.Id, newTranscript);
				if(resultSaveWords != null)
				{
					return new VersionDTO { Version = null, Error = "Error updating new version with id: " + newVersion.Id };
				}
			}
			catch
			{
				return new VersionDTO { Version = null, Error = "Error saving new words with id: " + newVersion.Id };
			}

			//Find corresponding file and update its flag 
			try
			{
				File file = await _context.File.Include(q => q.Reviewer).FirstOrDefaultAsync( q => q.Id == newVersion.FileId);
				string flag = (file.Reviewer.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase) ? "Révisé" : "Edité"); //If user is reviewer of file, flag = "Révisé"
				file.Flag = flag;
				await _context.SaveChangesAsync();
				//Send email to user who uploaded file stating that review is done
				if (flag == "Révisé")
				{
					var uploader = await _context.User.FindAsync(file.UserId);
					var reviewer = await _context.User.FindAsync(file.ReviewerId);
					var emailSerice = new EmailInfrastructure();
					emailSerice.SendReviewDoneEmail(uploader.Email, file, reviewer.Name);

				}

				return new VersionDTO { Version = newVersion, Error = null };
			}
			catch
			{
				return new VersionDTO { Version = null, Error = "File flag not updated." };
			}
		}

		public async Task<string> SearchTranscript(string searchTerms, int versionId)
		{
			//Ordered by Id to get the words in the same order as transcript
			var words = await _context.Word.Where(w => w.VersionId == versionId).OrderBy(w => w.Id).ToListAsync();
			var searchService = new SearchService();
			return searchService.PerformSearch(searchTerms, words);
		}

		public async Task<string> DownloadTranscription(string documentType, int fileId)
		{
			var version = _context.Version.Where(v => v.FileId == fileId).Where(v => v.Active == true).SingleOrDefault(); //Gets the active version (last version of transcription)
			var rawTranscript = version.Transcription;
			var transcript = rawTranscript.Replace("<br>", "\n ");

			var exportResult = await Task.Run(async () => {

				if (documentType == "doc")
				{
					var wordRepository = new WordRepository();
					return wordRepository.CreateWordDocument(transcript);
				}
				else if (documentType == "googleDoc")
				{
					var googleDocRepository = new GoogleDocumentRepository();
					return googleDocRepository.CreateGoogleDocument(transcript);
				}
				else if (documentType == "srt")
				{
					var words = await _context.Word.Where(v => v.VersionId == version.Id).ToListAsync();
					if (words.Count > 0)
					{
						var exportTranscriptionService = new ExportTranscriptionService();
						return exportTranscriptionService.CreateSRTDocument(transcript, words);
					}
					else
						return false;
				}
				else
				{
					return false;
				}
			});

			if (exportResult)
			{
				return null;
			}
			else
			{
				return "Error while trying to download transcription";
			}
		}

		/// <summary>
		/// Private method that handles saving new words in the database when SaveTranscript is called
		/// This makes the transcript still searchable after adding new words
		/// </summary>
		private async Task<string> SaveWords(int versionId, int newVersionId, string newTranscript)
		{
			//Have to explicitly instantiate variable to be able to keep the words.
			List<Word> oldWords = new List<Word>();

			//Getting all the words for this versionId
			try
			{
				//Ordered by Id to get the words in the same order as transcript
				oldWords = await _context.Word.Where(w => w.VersionId == versionId).OrderBy(w => w.Id).ToListAsync();

			}
			catch
			{
				return "Error fetching words with versionId: " + versionId;
			}

			//Modify timestamps and return the new words
			var modifyTimeStampService = new ModifyTimeStampService();
			List<Word> newWords = modifyTimeStampService.ModifyTimestamps(oldWords, newTranscript, newVersionId);

			try
			{
				await _context.Word.AddRangeAsync(newWords);
				await _context.SaveChangesAsync();
			}
			catch
			{
				return "Error adding words with versionId: " + newVersionId;
			}
			return null;
		}
	}
}
