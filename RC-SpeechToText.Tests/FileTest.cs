
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using RC_SpeechToText.Models.DTO.Incoming;
using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace RC_SpeechToText.Tests
{
    public class FileTest
    {
		/// <summary>
		/// Test fetching all the files
		/// </summary>
		[Fact]
        public async Task TestGetAllVideos()
        {
            // Arrange
			var configuration = new ConfigurationBuilder()
			   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
			   .AddJsonFile("appsettings.json", false)
			   .Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());

			var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            await context.File.AddRangeAsync(Enumerable.Range(1, 20).Select(t => new File { Title = "Video " + t, FilePath = "vPath " + t }));
            await context.SaveChangesAsync();

			//Act
            var fileService = new FileService(context, config.Value);
            var result = await fileService.GetAllFiles();
                
            // Assert
            Assert.IsType<List<File>>(result);
            Assert.Equal(20, result.Count()); 
        }

        /// <summary>
        /// Test fetching all the files with the corresponding usernames
        /// </summary>
        [Fact]
        public async Task TestGetAllWithUsernames()
        {
            // Arrange
			var configuration = new ConfigurationBuilder()
			   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
			   .AddJsonFile("appsettings.json", false)
			   .Build();

            var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());
			var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Email = "test@email.com", Name = "testUser" };
           
            // Add user with username testUser
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            // Remove all files in DB
            //var files = await context.File.ToListAsync();
            //context.File.RemoveRange(files);
            //await context.SaveChangesAsync();

			// Add file with title testFile
			var newFile = new File { Title = "testFile", UserId = user.Id };
			await context.File.AddAsync(newFile);
            await context.SaveChangesAsync();

            // Act
            var fileService = new FileService(context, config.Value);
            var result = await fileService.GetAllWithUsernames();

            // Assert
            Assert.IsType<FileUsernameDTO>(result);

			var mockFileUsernameDTO = new FileUsernameDTO { Files = new List<File> { newFile }, Usernames = new List<string> { user.Name } };

            // Assert that username is testUser
            string username = mockFileUsernameDTO.Usernames[0];
            Assert.Equal("testUser", username);

            // Assert that title is testFile
            string fileTitle = mockFileUsernameDTO.Files[0].Title;
            Assert.Equal("testFile", fileTitle);
        }

        /// <summary>
        /// Test fetching all the files of a specific user
        /// </summary>
        [Fact]
        public async Task TestGetUserFiles()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Email = "test@email.com", Name = "testUser" };

            // Add user with username testUser
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            // Remove all files in DB
            var files = await context.File.ToListAsync();
            context.File.RemoveRange(files);
            await context.SaveChangesAsync();


            var file1 = new File { Title = "testFile1", UserId = user.Id };
            var file2 = new File { Title = "testFile2", UserId = user.Id };
            var file3 = new File { Title = "testFile3", UserId = user.Id };

            // Add files using userId
            await context.File.AddAsync(file1);
            await context.File.AddAsync(file2);
            await context.File.AddAsync(file3);
            await context.SaveChangesAsync();

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());

            // Act
            var fileService = new FileService(context, config.Value);
            var result = await fileService.GetAllFilesByEmail(user.Email);

            // Assert
            Assert.IsType<FileUsernameDTO>(result);

			var mockFileUsernameDTO = new FileUsernameDTO { Files = new List<File> { file1, file2, file3 }, Usernames = new List<string> { user.Name } };

			// Assert that username is testUser for all files
			for (int i = 0; i < mockFileUsernameDTO.Usernames.Count; i++)
            {
                string username = mockFileUsernameDTO.Usernames[i];
                Assert.Equal("testUser", username);
            }

            //Verify that all files has same userId as the user 
            for (int i = 0; i < mockFileUsernameDTO.Files.Count; i++)
            {
                Guid actualUserId = mockFileUsernameDTO.Files[i].UserId;
                Assert.Equal(user.Id, actualUserId);
            }
        }

        /// <summary>
        /// Test fetching one File by Id
        /// </summary>
        [Fact]
        public async Task TestDetails()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());
            
            var file = new File { Title = "testFile" };

            // Add file with title testFile
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());

			// Act
            var fileService = new FileService(context, config.Value);
            var result = await fileService.GetFileById(file.Id);

            // Assert
            Assert.IsType<File>(result);
        }

        /// <summary>
        /// Test asking another user to review a file
        /// </summary>
        [Fact]
        public async Task TestAskReview()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Email = "reviewer@email.com", Name = "testReviewer" };

            // Add user and reviewer with username testUser and testReviewer
            await context.AddAsync(user);
            await context.AddAsync(reviewer);
            await context.SaveChangesAsync();

            // Remove all files in DB
            var files = await context.File.ToListAsync();
            context.File.RemoveRange(files);
            await context.SaveChangesAsync();

            var file = new File { Title = "testFile1", UserId = user.Id };

            // Add files using userId
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();

			var configuration = new ConfigurationBuilder()
				.SetBasePath(System.IO.Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			var config = Options.Create(configuration.GetSection("someService").Get<AppSettings>());

			// Act
            var fileService = new FileService(context, config.Value);
            var result = await fileService.AddReviewer(file.Id, user.Email, reviewer.Email);
            
            // Assert
            Assert.IsType<FileDTO>(result);

            //Assert that reviewer has been added to the file
            Assert.Equal(file.ReviewerId, reviewer.Id);
        }

    }
}
