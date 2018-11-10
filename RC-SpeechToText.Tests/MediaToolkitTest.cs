using System;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RC_SpeechToText.Controllers;

namespace RC_SpeechToText.Tests
{
    [TestClass]
    public class MediaToolkitTest
    {

        public MediaFile FileConvert(string inputLocation, string outputLocation)
        {
            
            var inputFile = new MediaFile { Filename = inputLocation };
            var outputFile = new MediaFile { Filename = outputLocation };

            var conversionOptions = new ConversionOptions
            {
                AudioSampleRate = AudioSampleRate.Hz22050
            };

            try
            {
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    engine.Convert(inputFile, outputFile, conversionOptions);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return outputFile;
        }
        
        public Metadata GetMetadata(string fileLocation)
        {
            
            var inputFile = new MediaFile { Filename = fileLocation };

            try
            {
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return inputFile.Metadata;
        }
        
        [TestMethod]
        public void TestWavToMp3()
        {
            var inputFile = "..\\..\\..\\..\\RC-SpeechToText.Tests\\TestFiles\\demo.wav";
            var outputFile = "..\\..\\..\\..\\RC-SpeechToText.Tests\\TestFiles\\demo.mp3";

            // Delete pre-existing mp3 file
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            //Create new mp3 using conversion
            var convertedFile = FileConvert(inputFile, outputFile);


            var wavMetadata = GetMetadata(inputFile);
            var mp3Metadata = GetMetadata(outputFile);

            
            Assert.IsNotNull(convertedFile);
            Assert.AreEqual(mp3Metadata.AudioData.SampleRate, "22050 Hz");
            Assert.AreEqual(mp3Metadata.AudioData.Format, "mp3");
            Assert.AreEqual(mp3Metadata.AudioData.SampleRate, wavMetadata.AudioData.SampleRate);
        }
        
        [TestMethod]
        public void TestMp3ToWav()
        {
            var inputFile = "..\\..\\..\\..\\RC-SpeechToText.Tests\\TestFiles\\demo.mp3";
            var outputFile = "..\\..\\..\\..\\RC-SpeechToText.Tests\\TestFiles\\demo.wav";
            
            //Delete pre-existing wav file
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            //Create new wav using conversion
            var convertedFile = FileConvert(inputFile, outputFile);

            var mp3Metadata = GetMetadata(inputFile);
            var wavMetadata = GetMetadata(outputFile);

            Assert.IsNotNull(convertedFile);
            Assert.AreEqual(wavMetadata.AudioData.SampleRate, "22050 Hz");
            Assert.AreEqual(wavMetadata.AudioData.Format, "pcm_s16le ([1][0][0][0] / 0x0001)");
            Assert.AreEqual(mp3Metadata.AudioData.SampleRate, wavMetadata.AudioData.SampleRate);
        }
        
        [TestMethod]
        public void TestMp3ToFlac()
        {
            var inputFile = "..\\..\\..\\..\\RC-SpeechToText.Tests\\TestFiles\\demo.mp3";
            var outputFile = "..\\..\\..\\..\\RC-SpeechToText.Tests\\TestFiles\\demo.flac";
            
            //Delete pre-existing flac file
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            //Create new flac using conversion
            var convertedFile = FileConvert(inputFile, outputFile);

            var mp3Metadata = GetMetadata(inputFile);
            var wavMetadata = GetMetadata(outputFile);

            Assert.IsNotNull(convertedFile);
            Assert.AreEqual(wavMetadata.AudioData.SampleRate, "22050 Hz");
            Assert.AreEqual(wavMetadata.AudioData.Format, "flac");
            Assert.AreEqual(mp3Metadata.AudioData.SampleRate, wavMetadata.AudioData.SampleRate);
        }
    }
}
