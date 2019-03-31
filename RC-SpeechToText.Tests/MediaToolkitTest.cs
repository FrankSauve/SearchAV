using System;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RC_SpeechToText.Utils;

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

        public void GenericConversionTester(string inputType, string outputType)
        {
            var inputFile = "..\\..\\..\\..\\RC-SpeechToText.Tests\\TestFiles\\demo." + inputType;
            var outputFile = "..\\..\\..\\..\\RC-SpeechToText.Tests\\TestFiles\\demo." + outputType;

            // Delete pre-existing output file
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            //Create new output file using conversion
            var convertedFile = FileConvert(inputFile, outputFile);


            var inputFileMetadata = GetMetadata(inputFile);
            var outputFileMetadata = GetMetadata(outputFile);

            
            Assert.IsNotNull(convertedFile);
            Assert.AreEqual(outputFileMetadata.AudioData.SampleRate, "22050 Hz");
            Assert.AreEqual(outputFileMetadata.AudioData.Format,
                outputType != "wav" ? outputType : "pcm_s16le ([1][0][0][0] / 0x0001)");
            Assert.AreEqual(outputFileMetadata.AudioData.SampleRate, inputFileMetadata.AudioData.SampleRate);
        }
        
        [TestMethod]
        public void TestFlacToMp3()
        {
            GenericConversionTester("flac","mp3");
        }
        [TestMethod]
        public void TestFlacToWav()
        {
            GenericConversionTester("flac", "wav");
        }
        
        [TestMethod]
        public void TestMp3ToWav()
        {
            GenericConversionTester("mp3","wav");
        }
        [TestMethod]
        public void TestMp3ToFlac()
        {
            GenericConversionTester("mp3", "flac");
        }
        
        [TestMethod]
        public void TestWavToFlac()
        {
            GenericConversionTester("wav", "flac");
        }
        [TestMethod]
        public void TestWavToMp3()
        {
            GenericConversionTester("wav","mp3");
        }

        [TestMethod]
        public void TestCreateThumbnail()
        {
            // Arrange
            var inputFile = @"..\..\..\..\RC-SpeechToText.Tests\TestFiles\demo.mp4";
            var outputFile = @"..\..\..\..\RC-SpeechToText.Tests\TestFiles\demo.jpg";
            var converter = new Converter();
            
            // Act
            converter.CreateThumbnail(inputFile, outputFile, 1000);

            var thumbnailPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), outputFile));
            
            // Assert
            Assert.IsTrue(File.Exists(thumbnailPath));

            // Delete the thumbnail
            File.Delete(thumbnailPath);
        }
    }
}
