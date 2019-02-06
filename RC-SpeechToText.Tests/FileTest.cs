
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
            // Arrange
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
            var context = new SearchAVContext(options);

            await context.File.AddRangeAsync(Enumerable.Range(1, 20).Select(t => new File { Title = "Video " + t, FilePath = "vPath " + t }));
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new FileController(context, logger);

            //Act
            var result = await controller.Index();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<File>>(okResult.Value);
            Assert.True(returnValue.Count() == 20);

        }

        /// <summary>
        /// Test fetching all the files with the corresponding usernames
        /// </summary>
        [Fact]
        public async Task TestGetAllWithUsernames()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
            var context = new SearchAVContext(options);

            // Add user with username testUser
            var user = new User { Email = "test@email.com", Name = "testUser" };
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            // Remove all files in DB
            var files = await context.File.ToListAsync();
            context.File.RemoveRange(files);
            await context.SaveChangesAsync();

            // Add file with title testFile
            await context.File.AddAsync(new File { Title = "testFile", UserId = user.Id });
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new FileController(context, logger);

            // Act
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
            // Arrange
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
            var context = new SearchAVContext(options);

            // Add user with username testUser
            var user = new User { Email = "test@email.com", Name = "testUser" };
            await context.AddAsync(user);
            user.Id = await context.SaveChangesAsync();

            // Remove all files in DB
            var files = await context.File.ToListAsync();
            context.File.RemoveRange(files);
            await context.SaveChangesAsync();

            // Add files using userId
            await context.File.AddAsync(new File { Title = "testFile1", UserId = user.Id });
            await context.File.AddAsync(new File { Title = "testFile2", UserId = user.Id });
            await context.File.AddAsync(new File { Title = "testFile3", UserId = user.Id });
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new FileController(context, logger);

            // Act
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
                Assert.Equal(user.Name, username);
            }

            //Verify that all files has same userId as the user 
            for (int i = 0; i < data.Value.files.Count; i++)
            {
                int userId = data.Value.files[i].UserId;
                Assert.Equal(user.Id, userId);
            }

        }

        ///// <summary>
        ///// Test fetching all the automated files 
        ///// </summary>
        //[Fact]
        //public async Task TestGetAutomatedFiles()
        //{
        //    // Arrange
        //    var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
        //    var context = new SearchAVContext(options);

        //    // Remove all files in DB
        //    var files = await context.File.ToListAsync();
        //    context.File.RemoveRange(files);
        //    await context.SaveChangesAsync();

        //    // Add user with username testUser
        //    var user = new User { Email = "test@email.com", Name = "testUser" };
        //    await context.AddAsync(user);
        //    user.Id = await context.SaveChangesAsync();

        //    // Add files using flag
        //    await context.File.AddAsync(new File { Title = "testFile1", UserId = user.Id, Flag = "Automatisé" });
        //    await context.File.AddAsync(new File { Title = "testFile2", UserId = user.Id, Flag = "Automatisé" });
        //    await context.File.AddAsync(new File { Title = "testFile3", UserId = user.Id }); //No flag for testing purposes
        //    await context.SaveChangesAsync();

        //    var mock = new Mock<ILogger<FileController>>();
        //    ILogger<FileController> logger = mock.Object;
        //    logger = Mock.Of<ILogger<FileController>>();

        //    var controller = new FileController(context, logger);

        //    // Act
        //    var result = await controller.getAllAutomatedFiles();

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var returnValue = Assert.IsType<JsonResult>(okResult.Value);

        //    var json = JsonConvert.SerializeObject(okResult.Value);
        //    dynamic data = JsonConvert.DeserializeObject<object>(json);

        //    // Verify that we get 2 files 
        //    int automatedFiles = data.Value.files.Count;
        //    Assert.Equal(2, automatedFiles);

        //    //Verify the flags
        //    for (int i = 0; i < automatedFiles; i++)
        //    {
        //        string flag = data.Value.files[i].Flag;
        //        Assert.Equal("Automatisé", flag);
        //    }

        //}

        ///// <summary>
        ///// Test fetching all the edited files 
        ///// </summary>
        //[Fact]
        //public async Task TestGetEditedFiles()
        //{
        //    // Arrange
        //    var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
        //    var context = new SearchAVContext(options);

        //    // Remove all files in DB
        //    var files = await context.File.ToListAsync();
        //    context.File.RemoveRange(files);
        //    await context.SaveChangesAsync();

        //    // Add user with username testUser
        //    var user = new User { Email = "test@email.com", Name = "testUser" };
        //    await context.AddAsync(user);
        //    user.Id = await context.SaveChangesAsync();

        //    // Add files using flag
        //    await context.File.AddAsync(new File { Title = "testFile1", UserId = user.Id, Flag = "Edité" });
        //    await context.File.AddAsync(new File { Title = "testFile2", UserId = user.Id, Flag = "Edité" });
        //    await context.File.AddAsync(new File { Title = "testFile3", UserId = user.Id }); //No flag for testing purposes
        //    await context.SaveChangesAsync();

        //    var mock = new Mock<ILogger<FileController>>();
        //    ILogger<FileController> logger = mock.Object;
        //    logger = Mock.Of<ILogger<FileController>>();

        //    var controller = new FileController(context, logger);

        //    // Act
        //    var result = await controller.getAllEditedFiles();

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var returnValue = Assert.IsType<JsonResult>(okResult.Value);

        //    var json = JsonConvert.SerializeObject(okResult.Value);
        //    dynamic data = JsonConvert.DeserializeObject<object>(json);

        //    // Verify that we get 2 files 
        //    int editedFiles = data.Value.files.Count;
        //    Assert.Equal(2, editedFiles);

        //    //Verify the flags
        //    for (int i = 0; i < editedFiles; i++)
        //    {
        //        string flag = data.Value.files[i].Flag;
        //        Assert.Equal("Edité", flag);
        //    }

        //}

        ///// <summary>
        ///// Test fetching all the reviewed files 
        ///// </summary>
        //[Fact]
        //public async Task TestGetReviewedFiles()
        //{
        //    // Arrange
        //    var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
        //    var context = new SearchAVContext(options);

        //    // Remove all files in DB
        //    var files = await context.File.ToListAsync();
        //    context.File.RemoveRange(files);
        //    await context.SaveChangesAsync();

        //    // Add user with username testUser
        //    var user = new User { Email = "test@email.com", Name = "testUser" };
        //    await context.AddAsync(user);
        //    user.Id = await context.SaveChangesAsync();

        //    // Add files using flag
        //    await context.File.AddAsync(new File { Title = "testFile1", UserId = user.Id, Flag = "Révisé" });
        //    await context.File.AddAsync(new File { Title = "testFile2", UserId = user.Id, Flag = "Révisé" });
        //    await context.File.AddAsync(new File { Title = "testFile3", UserId = user.Id }); //No flag for testing purposes
        //    await context.SaveChangesAsync();

        //    var mock = new Mock<ILogger<FileController>>();
        //    ILogger<FileController> logger = mock.Object;
        //    logger = Mock.Of<ILogger<FileController>>();

        //    var controller = new FileController(context, logger);

        //    // Act
        //    var result = await controller.getAllReviewedFiles();

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var returnValue = Assert.IsType<JsonResult>(okResult.Value);

        //    var json = JsonConvert.SerializeObject(okResult.Value);
        //    dynamic data = JsonConvert.DeserializeObject<object>(json);

        //    // Verify that we get 2 files 
        //    int reviewedFiles = data.Value.files.Count;
        //    Assert.Equal(2, reviewedFiles);

        //    //Verify the flags
        //    for (int i = 0; i < reviewedFiles; i++)
        //    {
        //        string flag = data.Value.files[i].Flag;
        //        Assert.Equal("Révisé", flag);
        //    }

        //}

        /// <summary>
        /// Test fetching one File by Id
        /// </summary>
        [Fact]
        public async Task TestDetails()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SearchAVContext>().UseInMemoryDatabase().Options;
            var context = new SearchAVContext(options);

            // Add file with title testFile
            var file = new File { Title = "testFile" };
            await context.File.AddAsync(file);
            await context.SaveChangesAsync();

            var mock = new Mock<ILogger<FileController>>();
            ILogger<FileController> logger = mock.Object;
            logger = Mock.Of<ILogger<FileController>>();

            var controller = new FileController(context, logger);

            // Act
            var result = await controller.Details(file.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<File>(okResult.Value);
        }
    }
}
