using System;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Xunit;
using RC_SpeechToText.Utils;

namespace RC_SpeechToText.Tests
{
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

            
            Assert.NotNull(convertedFile);
            Assert.Equal("22050 Hz", outputFileMetadata.AudioData.SampleRate);
            Assert.Equal(outputType != "wav" ? outputType : "pcm_s16le ([1][0][0][0] / 0x0001)",
                outputFileMetadata.AudioData.Format);
            Assert.Equal(outputFileMetadata.AudioData.SampleRate, inputFileMetadata.AudioData.SampleRate);
        }
        
        [Fact]
        public void TestFlacToMp3()
        {
            GenericConversionTester("flac","mp3");
        }
        [Fact]
        public void TestFlacToWav()
        {
            GenericConversionTester("flac", "wav");
        }
        
        [Fact]
        public void TestMp3ToWav()
        {
            GenericConversionTester("mp3","wav");
        }
        [Fact]
        public void TestMp3ToFlac()
        {
            GenericConversionTester("mp3", "flac");
        }
        
        [Fact]
        public void TestWavToFlac()
        {
            GenericConversionTester("wav", "flac");
        }
        [Fact]
        public void TestWavToMp3()
        {
            GenericConversionTester("wav","mp3");
        }

        [Fact]
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
            Assert.True(File.Exists(thumbnailPath));

            // Delete the thumbnail
            File.Delete(thumbnailPath);
        }
    }
}
