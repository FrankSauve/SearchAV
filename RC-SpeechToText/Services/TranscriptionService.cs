using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Exceptions;
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

            _context.Version.Update(currentVersion);
            await _context.SaveChangesAsync();


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

            await _context.Version.AddAsync(newVersion);
            await _context.SaveChangesAsync();


            //Calling this method will handle saving the new words in the databse
            var resultSaveWords = await SaveWords(versionId, newVersion.Id, newTranscript);



            //flag -> Edité
            var editedFlag = Enum.GetName(typeof(FileFlag), 1);

            //flag -> Révisé
            var reviewedFlag = Enum.GetName(typeof(FileFlag), 2);

            //Find corresponding file and update its flag 
            File file = await _context.File.Include(q => q.Reviewer).FirstOrDefaultAsync(q => q.Id == newVersion.FileId);
            string flag = (file.Reviewer.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase) ? reviewedFlag : editedFlag); //If user is reviewer of file, flag = "Révisé"
            file.Flag = flag;
            await _context.SaveChangesAsync();
            //Send email to user who uploaded file stating that review is done
            if (flag == reviewedFlag)
            {
                var uploader = await _context.User.FindAsync(file.UserId);
                var reviewer = await _context.User.FindAsync(file.ReviewerId);
                var emailSerice = new EmailInfrastructure();
                emailSerice.SendReviewDoneEmail(uploader.Email, file, reviewer.Name);

            }

            return new VersionDTO { Version = newVersion, Error = null };

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

            var exportResult = await Task.Run(async () =>
            {

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

            //Ordered by Id to get the words in the same order as transcript
            oldWords = await _context.Word.Where(w => w.VersionId == versionId).OrderBy(w => w.Id).ToListAsync();



            //Modify timestamps and return the new words
            var modifyTimeStampService = new ModifyTimeStampService();
            List<Word> newWords = modifyTimeStampService.ModifyTimestamps(oldWords, newTranscript, newVersionId);


            await _context.Word.AddRangeAsync(newWords);
            await _context.SaveChangesAsync();

            return null;
        }
    }
}
