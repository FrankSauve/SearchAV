﻿using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Infrastructure;
using RC_SpeechToText.Models;
using RC_SpeechToText.Models.DTO.Incoming;
using RC_SpeechToText.Models.DTO.Outgoing;
using RC_SpeechToText.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services {
    public class TranscriptionService {
        private readonly SearchAVContext _context;
		private readonly AppSettings _appSettings;

		public TranscriptionService(SearchAVContext context, AppSettings appSettings)
        {
            _context = context;
			_appSettings = appSettings;
		}

        public async Task<VersionDTO> SaveTranscript(string userEmail, Guid versionId, string newTranscript)
        {
            var newVersion = await CreateNewVersion(versionId, newTranscript, userEmail);
            var duration = await _context.File.Where(f => f.Id == newVersion.FileId).Select(f => f.Duration).FirstOrDefaultAsync();
            var resultSaveWords = await SaveWords(versionId, newVersion.Id, newTranscript, duration);

            if (resultSaveWords != null)
			{
				return new VersionDTO { Version = null, Error = "Error updating new version with id: " + newVersion.Id };
			}

            var file = await UpdateFileFlag(userEmail, newVersion);

            if (file.FileFlag == FileFlag.Revise)
			{
				await EmailReviewer(file);
				newVersion.HistoryTitle = "FICHIER RÉVISÉ"; //If user is reviewer of file, historyTitle = "FICHIER REVISE"
			}

			return new VersionDTO { Version = newVersion, Error = null };
		}

		public async Task<string> SearchTranscript(OutSearchTranscriptDTO outSearchTranscriptDTO)
		{
			//Ordered by Id to get the words in the same order as transcript
			var words = await _context.Word.Where(w => Guid.Equals(w.VersionId, outSearchTranscriptDTO.VersionId)).OrderBy(w => w.Position).ToListAsync();
			var searchService = new SearchService();
			return searchService.PerformSearch(outSearchTranscriptDTO.SearchTerms, words);
		}

		public async Task<string> PrepareDownload(OutDownloadTranscriptDTO outDownloadTranscriptDTO)
		{
			var fileTitle = _context.File.Where(x => x.Id == outDownloadTranscriptDTO.FileId).Select(x => x.Title).SingleOrDefault();
			var version = _context.Version.Where(v => v.FileId == outDownloadTranscriptDTO.FileId).Where(v => v.Active == true).SingleOrDefault(); //Gets the active version (last version of transcription)
			var rawTranscript = version.Transcription;
			var transcript = rawTranscript.Replace("<br>", "\n ");

            var exportResult = await Task.Run(async () =>
			{
				return await GenerateDocument(outDownloadTranscriptDTO, fileTitle, version, transcript);
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

		private async Task<bool> GenerateDocument(OutDownloadTranscriptDTO outDownloadTranscriptDTO, string fileTitle, Models.Version version, string transcript)
		{
			if (outDownloadTranscriptDTO.DocumentType == DocumentType.Doc)
			{
				return GenerateWordDoucment(transcript);
			}
			else if (outDownloadTranscriptDTO.DocumentType == DocumentType.GoogleDoc)
			{
				return GenerateGoogleDocument(fileTitle, transcript);
			}
			else if (outDownloadTranscriptDTO.DocumentType == DocumentType.Srt)
			{
				return await GenerateSRTDocument(fileTitle, version, transcript);
			}
			else if (outDownloadTranscriptDTO.DocumentType.Contains(DocumentType.Video))
			{
				return await GenerateVideoWithSubtitle(outDownloadTranscriptDTO, fileTitle, version, transcript);
			}
			else
			{
				return false;
			}
		}

		private async Task<bool> GenerateVideoWithSubtitle(OutDownloadTranscriptDTO outDownloadTranscriptDTO, string fileTitle, Models.Version version, string transcript)
		{
			var words = await _context.Word.Where(v => Guid.Equals(v.VersionId, version.Id)).OrderBy(v => v.Position).ToListAsync();
			if (words.Count > 0)
			{
				var exportTranscriptionService = new ExportTranscriptionService(_context, _appSettings);
				return await exportTranscriptionService.ExportVideo(fileTitle, outDownloadTranscriptDTO.DocumentType, transcript, words);
			}
			else
				return false;
		}

		private async Task<bool> GenerateSRTDocument(string fileTitle, Models.Version version, string transcript)
		{
			var words = await _context.Word.Where(v => Guid.Equals(v.VersionId, version.Id)).OrderBy(v => v.Position).ToListAsync();
			if (words.Count > 0)

			{
				var exportTranscriptionService = new ExportTranscriptionService(_context, _appSettings);
				return exportTranscriptionService.CreateSRTDocument(transcript, words, fileTitle);
			}
			else
				return false;
		}

		private static bool GenerateGoogleDocument(string fileTitle, string transcript)
		{
			var googleDocRepository = new GoogleDocumentRepository();
			return googleDocRepository.CreateGoogleDocument(transcript, fileTitle);
		}

		private static bool GenerateWordDoucment(string transcript)
		{
			var wordRepository = new WordRepository();
			return wordRepository.CreateWordDocument(transcript);
		}

		private async Task EmailReviewer(File file)
		{
			var uploader = await _context.User.FindAsync(file.UserId);
			var reviewer = await _context.User.FindAsync(file.ReviewerId);
			var emailService = new EmailInfrastructure();
			emailService.SendReviewDoneEmail(uploader.Email, file, reviewer.Name);
		}

		private async Task<File> UpdateFileFlag(string userEmail, Models.Version newVersion)
		{
			//Find corresponding file and update its flag 
			var file = await _context.File.Include(q => q.Reviewer).FirstOrDefaultAsync(q => q.Id == newVersion.FileId);

			FileFlag flag;
			if (file != null)
				flag = (file.Reviewer.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase) ? FileFlag.Revise : FileFlag.Edite); //If user is reviewer of file, flag = "Révisé"
			else
			{
				file = await _context.File.Where(q => q.Id == newVersion.FileId).FirstOrDefaultAsync();
				flag = FileFlag.Edite;
			}

			file.FileFlag = flag;

			await _context.SaveChangesAsync();

			return file;
		}

		/// <summary>
		/// Private method that handles saving new words in the database when SaveTranscript is called
		/// This makes the transcript still searchable after adding new words
		/// </summary>
		private async Task<string> SaveWords(Guid versionId, Guid newVersionId, string newTranscript, string duration)
        {
            //Have to explicitly instantiate variable to be able to keep the words.
            List<Word> oldWords = new List<Word>();

            //Getting all the words for this versionId

            //Ordered by position to get the words in the same order as transcript
            oldWords = await _context.Word.Where(w => w.VersionId == versionId).OrderBy(w => w.Position).ToListAsync();
            var oldTranscript =  _context.Version.Where(v => v.Id == versionId).Select(v => v.Transcription).SingleOrDefault();
            //Modify timestamps and return the new words
            var modifyTimeStampService = new ModifyTimeStampService();
            List<Word> newWords = modifyTimeStampService.ModifyTimestamps(oldWords, oldTranscript.ClearHTMLTag(), newTranscript.ClearHTMLTag(), newVersionId, duration);

            newWords.ForEach(async x =>
            {
                await _context.Word.AddAsync(x);
            });
            await _context.SaveChangesAsync();
                    
            return null;
        }

        private async Task<Models.Version> CreateNewVersion(Guid versionId, string newTranscript, string userEmail) {
            var currentVersion = _context.Version.Find(versionId);

            //Deactivate current version 
            currentVersion.Active = false;

            //Capitalize the first letter when saving the transcript
            newTranscript = newTranscript.First().ToString().ToUpper() + newTranscript.Substring(1);

            //Create a new version
            var newVersion = new Models.Version
			    {
				    UserId = await _context.User.Where(u => u.Email == userEmail).Select(u => u.Id).FirstOrDefaultAsync(),
				    FileId = currentVersion.FileId,
				    DateModified = DateTime.Now,
                    HistoryTitle = "MODIFICATIONS",
                    Transcription = newTranscript,
				    Active = true
			    };

            //Add new version to DB

            await _context.Version.AddAsync(newVersion);
            await _context.SaveChangesAsync();
            return newVersion;
        }
    }
}
