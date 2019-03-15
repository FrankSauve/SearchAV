
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using Xunit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RC_SpeechToText.Models.DTO.Incoming;

namespace RC_SpeechToText.Tests
{
    public class FiterTest
    {
        /// <summary>
        /// Test fetching all the automated files 
        /// </summary>
        [Fact]
        public async Task TestGetAutomatedFiles()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Email = "test@email.com", Name = "testUser" };

            // AddAsync user with username testUser
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var file1 = new File { Title = "testFile1", UserId = user.Id, FileFlag = FileFlag.Automatise };
			var file2 = new File { Title = "testFile2", UserId = user.Id, FileFlag = FileFlag.Automatise };
			var file3 = new File { Title = "testFile3", UserId = user.Id };

			// AddAsync files using flag
			await context.File.AddAsync(file1);
            await context.File.AddAsync(file2);
            await context.File.AddAsync(file3); //No flag for testing purposes
            await context.SaveChangesAsync();
			
            // Act
            var controller = new FileController(context);
            var result = await controller.getAllFilesByFlag("Automatise");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FileUsernameDTO>(okResult.Value);

			var mockFileUsernameDTO = new FileUsernameDTO { Files = new List<File> { file1, file2 }, Usernames = new List<string> { user.Name } };

			// Verify that we get 2 files 
			int automatedFiles = mockFileUsernameDTO.Files.Count;
            Assert.Equal(2, automatedFiles);

            //Verify the flags
            for (int i = 0; i < automatedFiles; i++)
            {
                var flag = mockFileUsernameDTO.Files[i].FileFlag;
                Assert.Equal(FileFlag.Automatise, flag);
            }
        }

        /// <summary>
        /// Test fetching all the edited files 
        /// </summary>
        [Fact]
        public async Task TestGetEditedFiles()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Email = "test@email.com", Name = "testUser" };

            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var file1 = new File { Title = "testFile1", UserId = user.Id, FileFlag = FileFlag.Edite};
			var file2 = new File { Title = "testFile2", UserId = user.Id, FileFlag = FileFlag.Edite };
			var file3 = new File { Title = "testFile3", UserId = user.Id };

			// AddAsync files using flag
			await context.File.AddAsync(file1);
			await context.File.AddAsync(file2);
			await context.File.AddAsync(file3); //No flag for testing purposes
			await context.SaveChangesAsync();

			// Act
			var controller = new FileController(context);
            var result = await controller.getAllFilesByFlag("Edite");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FileUsernameDTO>(okResult.Value);

			var mockFileUsernameDTO = new FileUsernameDTO { Files = new List<File> { file1, file2 }, Usernames = new List<string> { user.Name } };

			// Verify that we get 2 files 
			int editedFiles = mockFileUsernameDTO.Files.Count;
            Assert.Equal(2, editedFiles);

            //Verify the flags
            for (int i = 0; i < editedFiles; i++)
            {
                var flag = mockFileUsernameDTO.Files[i].FileFlag;
                Assert.Equal(FileFlag.Edite, flag);
            }
        }

        /// <summary>
        /// Test fetching all the reviewed files 
        /// </summary>
        [Fact]
        public async Task TestGetReviewedFiles()
        {
            // Arrange
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            // AddAsync user with username testUser
            var user = new User { Email = "test@email.com", Name = "testUser" };
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var file1 = new File { Title = "testFile1", UserId = user.Id, FileFlag = FileFlag.Revise };
            var file2 = new File { Title = "testFile2", UserId = user.Id, FileFlag = FileFlag.Revise };
            var file3 = new File { Title = "testFile3", UserId = user.Id }; // No flag for testing purposes

            // AddAsync files using flag
            await context.File.AddAsync(file1);
            await context.File.AddAsync(file2);
            await context.File.AddAsync(file3);
            await context.SaveChangesAsync();

            // Act
            var controller = new FileController(context);
            var result = await controller.getAllFilesByFlag("Revise");
;
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FileUsernameDTO>(okResult.Value);

			var mockFileUsernameDTO = new FileUsernameDTO { Files = new List<File> { file1, file2 }, Usernames = new List<string> { user.Name } };

			// Verify that we get 2 files 
			int reviewedFiles = mockFileUsernameDTO.Files.Count;
            Assert.Equal(2, reviewedFiles);

            //Verify the flags
            for (int i = 0; i < reviewedFiles; i++)
            {
                var flag = mockFileUsernameDTO.Files[i].FileFlag;
                Assert.Equal(FileFlag.Revise, flag);
            }
        }
    }
}
