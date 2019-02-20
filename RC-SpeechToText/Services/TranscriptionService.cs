using Google.Cloud.Speech.V1;
using Google.Cloud.Storage.V1;
using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    public class TranscriptionService
    {
        /// <summary>
        /// Transcribe the input file to text using Google Cloud
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <returns name="googleResult"></returns>
        public static LongRunningRecognizeResponse GoogleSpeechToText(string bucketName, string storageObjectName)
        {
            var speech = SpeechClient.Create();
            var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = "fr-ca",
                EnableWordTimeOffsets = true // Required to get timestamps
            }, RecognitionAudio.FromStorageUri($"gs://{bucketName}/{storageObjectName}"));

            longOperation = longOperation.PollUntilCompleted();
            var response = longOperation.Result;

            return response;
        }

        /// <summary>
        /// Uploads an object to a Google Cloud Bucket.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="localPath"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static async Task<Google.Apis.Storage.v1.Data.Object> UploadFile(string bucketName, string localPath, string objectName = null)
        {
            var storage = StorageClient.Create();
            using (var f = System.IO.File.OpenRead(localPath))
            {
                objectName = objectName ?? Path.GetFileName(localPath);
                return await storage.UploadObjectAsync(bucketName, objectName, null, f);
            }
        }

        /// <summary>
        /// Downloads an object from Google Cloud Bucket to the desired local path.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public static async Task DownloadObject(string bucketName, string objectName, string localPath = null)
        {
            var storage = StorageClient.Create();
            localPath = localPath ?? Path.GetFileName(objectName);
            using (var outputFile = System.IO.File.OpenWrite(localPath))
            {
                await storage.DownloadObjectAsync(bucketName, objectName, outputFile);
            }
        }

        /// <summary>
        /// Delete an object
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectNames"></param>
        public static async Task DeleteObject(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            await storage.DeleteObjectAsync(bucketName, objectName);
        }
    }
}
