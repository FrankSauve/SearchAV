
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

            // AddAsync files using flag
            await context.File.AddAsync(new File { Title = "testFile1", UserId = user.Id, Flag = "Automatisé" });
            await context.File.AddAsync(new File { Title = "testFile2", UserId = user.Id, Flag = "Automatisé" });
            await context.File.AddAsync(new File { Title = "testFile3", UserId = user.Id }); //No flag for testing purposes
            await context.SaveChangesAsync();
			
            // Act
            var controller = new FileController(context);
            var result = await controller.getAllAutomatedFiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<JsonResult>(okResult.Value);

            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic data = JsonConvert.DeserializeObject<object>(json);

            // Verify that we get 2 files 
            int automatedFiles = data.Value.files.Count;
            Assert.Equal(2, automatedFiles);

            //Verify the flags
            for (int i = 0; i < automatedFiles; i++)
            {
                string flag = data.Value.files[i].Flag;
                Assert.Equal("Automatisé", flag);
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

            // AddAsync files using flag
            await context.File.AddAsync(new File { Title = "testFile1", UserId = user.Id, Flag = "Edité" });
            await context.File.AddAsync(new File { Title = "testFile2", UserId = user.Id, Flag = "Edité" });
            await context.File.AddAsync(new File { Title = "testFile3", UserId = user.Id }); //No flag for testing purposes
            await context.SaveChangesAsync();

            // Act
            var controller = new FileController(context);
            var result = await controller.getAllEditedFiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<JsonResult>(okResult.Value);

            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic data = JsonConvert.DeserializeObject<object>(json);

            // Verify that we get 2 files 
            int editedFiles = data.Value.files.Count;
            Assert.Equal(2, editedFiles);

            //Verify the flags
            for (int i = 0; i < editedFiles; i++)
            {
                string flag = data.Value.files[i].Flag;
                Assert.Equal("Edité", flag);
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
            
            var file1 = new File { Title = "testFile1", UserId = user.Id, Flag = "Révisé" };
            var file2 = new File { Title = "testFile2", UserId = user.Id, Flag = "Révisé" };
            var file3 = new File { Title = "testFile3", UserId = user.Id }; // No flag for testing purposes

            // AddAsync files using flag
            await context.File.AddAsync(file1);
            await context.File.AddAsync(file2);
            await context.File.AddAsync(file3);
            await context.SaveChangesAsync();

            // Act
            var controller = new FileController(context);
            var result = await controller.getAllReviewedFiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<JsonResult>(okResult.Value);

            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic data = JsonConvert.DeserializeObject<object>(json);

            // Verify that we get 2 files 
            int reviewedFiles = data.Value.files.Count;
            Assert.Equal(2, reviewedFiles);

            //Verify the flags
            for (int i = 0; i < reviewedFiles; i++)
            {
                string flag = data.Value.files[i].Flag;
                Assert.Equal("Révisé", flag);
            }
        }
    }
}
