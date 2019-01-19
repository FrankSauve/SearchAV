using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace RC_SpeechToText.Tests
{
    public class UserTest
    {
        /// <summary>
        /// Makes a call to a protected route without a proper Google JWT Token
        /// Expects the HTTP status code to be 401 unauthorized
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UnauthorizedTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            var client = server.CreateClient();
            var url = "api/video/index";
            var expected = HttpStatusCode.Unauthorized;

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(expected, response.StatusCode);
        }
    }
}
