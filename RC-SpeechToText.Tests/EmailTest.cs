using System;
using System.Collections.Generic;
using System.Text;
using RC_SpeechToText.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RC_SpeechToText.Models;
using RC_SpeechToText.Services;

namespace RC_SpeechToText.Tests
{
    [TestClass]
    public class EmailTest
    {
        [TestMethod]
        public void TestSendTranscriptionDoneEmail()
        {
            // Arrange
            var user1 = new User { Email = "user1@email.com", Name = "testUser1" };
            var user2 = new User { Email = "user2email.com", Name = "testUser2" };
            var file1 = new File { Title = "testFile1", UserId = user1.Id, Id = 1, };
            //Act
            var emailService = new EmailService();
            //Assert
            Assert.IsTrue(emailService.SendTranscriptionDoneEmail(user1.Email, file1));
            Assert.IsFalse(emailService.SendTranscriptionDoneEmail(user2.Email, file1));
        }

        [TestMethod]
        public void SendReviewDoneEmail()
        {
            // Arrange
            var user1 = new User { Email = "user1@email.com", Name = "testUser1" };
            var user2 = new User { Email = "user2email.com", Name = "testUser2" };
            var reviewer = new User { Email = "reviewer@email.com", Name = "testReviewer" };
            var file1 = new File { Title = "testFile1", UserId = user1.Id, Id = 1, };
            File file2 = new File();
            //Act
            var emailService = new EmailService();
            //Assert
            Assert.IsTrue(emailService.SendReviewDoneEmail(user1.Email, file1, reviewer.Name));
            Assert.IsFalse(emailService.SendReviewDoneEmail(user2.Email, file1, reviewer.Name));
        }
    }
}
