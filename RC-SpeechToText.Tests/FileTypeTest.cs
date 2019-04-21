using RC_SpeechToText.Utils;
using Xunit;

namespace RC_SpeechToText.Tests
{
    public class FileTypeTest
    {
        [Fact]
        public void TestGetFileType()
        {
            // Arrange
            var inputFile1 = @"..\..\..\..\RC-SpeechToText.Tests\TestFiles\demo.mp4";
            var inputFile2 = @"..\..\..\..\RC-SpeechToText.Tests\TestFiles\demo.MP4";
            var inputFile3 = @"..\..\..\..\RC-SpeechToText.Tests\TestFiles\demo.flac";
            var inputFile4 = @"..\..\..\..\RC-SpeechToText.Tests\TestFiles\demo.FLAC";
            var inputFile5 = @"..\..\..\..\RC-SpeechToText.Tests\TestFiles\demo.wav";
            var inputFile6 = @"..\..\..\..\RC-SpeechToText.Tests\TestFiles\demo.tsx";
            var converter = new Converter();

            //Act
            string type1 = converter.GetFileType(inputFile1);
            string type2 = converter.GetFileType(inputFile2);
            string type3 = converter.GetFileType(inputFile3);
            string type4 = converter.GetFileType(inputFile4);
            string type5 = converter.GetFileType(inputFile5);
            string type6 = converter.GetFileType(inputFile6);

            //Assert
            Assert.Equal("Video", type1);
            Assert.Equal("Video", type2);
            Assert.Equal("Audio", type3);
            Assert.Equal("Audio", type4);
            Assert.Equal("Audio", type5);
            Assert.Equal("N/A", type6);
        }
    }
}
