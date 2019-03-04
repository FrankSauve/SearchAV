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
            var user = new User { Email = "user@email.com", Name = "testUser" };
            var user2 = new User { Email = "useremail.com", Name = "testUser" };
            var reviewer = new User { Email = "reviewer@email.com", Name = "testReviewer" };
            var file = new File { Title = "testFile1", UserId = user.Id, Id = 1, };
            //Act
            var emailService = new EmailService();
            
            //Assert
            Assert.IsTrue(emailService.SendTranscriptionDoneEmail(user.Email, file));
            Assert.IsFalse(emailService.SendTranscriptionDoneEmail(user2.Email, file));
        }
    }
}
