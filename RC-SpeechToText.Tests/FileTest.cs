
using RC_SpeechToText.Controllers;
using RC_SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            await context.File.AddRangeAsync(Enumerable.Range(1, 20).Select(t => new File { Title = "Video " + t, FilePath = "vPath " + t }));
            await context.SaveChangesAsync();

            //Act
            var controller = new FileController(context);
            var result = await controller.Index();
                
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<File>>(okResult.Value);
            Assert.Equal(20, returnValue.Count()); 
        }

        /// <summary>
        /// Test fetching all the files with the corresponding usernames
        /// </summary>
        [Fact]
        public async Task TestGetAllWithUsernames()
        {
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Email = "test@email.com", Name = "testUser" };

            // Add user with username testUser
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            // Remove all files in DB
            var files = await context.File.ToListAsync();
            context.File.RemoveRange(files);
            await context.SaveChangesAsync();

            // Add file with title testFile
            await context.File.AddAsync(new File { Title = "testFile", UserId = user.Id });
            await context.SaveChangesAsync();

            // Act
            var controller = new FileController(context);
            var result = await controller.GetAllWithUsernames();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<JsonResult>(okResult.Value);

            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic data = JsonConvert.DeserializeObject<object>(json);

            // Assert that username is testUser
            string username = data.Value.usernames[0];
            Assert.Equal("testUser", username);

            // Assert that title is testFile
            string fileTitle = data.Value.files[0].Title;
            Assert.Equal("testFile", fileTitle);
        }

        /// <summary>
        /// Test fetching all the files of a specific user
        /// </summary>
        [Fact]
        public async Task TestGetUserFiles()
        {
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

            // Act
            var controller = new FileController(context);

            var result = await controller.getAllFilesByUser(user.Id);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<JsonResult>(okResult.Value);

            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic data = JsonConvert.DeserializeObject<object>(json);

            // Assert that username is testUser for all files
            for (int i = 0; i < data.Value.usernames.Count; i++)
            {
                string username = data.Value.usernames[i];
                Assert.Equal("testUser", username);
            }

            //Verify that all files has same userId as the user 
            for (int i = 0; i < data.Value.files.Count; i++)
            {
                int actualUserId = data.Value.files[i].UserId;
                Assert.Equal(user.Id, actualUserId);
            }
        }

        /// <summary>
        /// Test fetching one File by Id
        /// </summary>
        [Fact]
        public async Task TestDetails()
        {
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());
            
            var file = new File { Title = "testFile" };

            // Add file with title testFile
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();

            // Act
            var controller = new FileController(context);
            var result = await controller.Details(file.Id);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<File>(okResult.Value);
        }

        /// <summary>
        /// Test asking another user to review a file
        /// </summary>
        [Fact]
        public async Task TestAskReview()
        {
            var context = new SearchAVContext(DbContext.CreateNewContextOptions());

            var user = new User { Email = "user@email.com", Name = "testUser" };
            var reviewer = new User { Email = "reviewer@email.com", Name = "testReviewer" };

            // Add user and reviewer with username testUser and testReviewer
            await context.AddAsync(user);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            // Remove all files in DB
            var files = await context.File.ToListAsync();
            context.File.RemoveRange(files);
            await context.SaveChangesAsync();


            var file = new File { Title = "testFile1", UserId = user.Id };

            // Add files using userId
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();
			
            // Act
            var controller = new FileController(context);
            var result = await controller.AddReviewer(file.Id, reviewer.Id);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<File>(okResult.Value);

            //Assert that reviewer has been added to the file
            Assert.Equal(file.ReviewerId, reviewer.Id);
        }

    }
}
