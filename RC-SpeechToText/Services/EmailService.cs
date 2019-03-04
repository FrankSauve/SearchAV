using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace RC_SpeechToText.Services
{
    public class EmailService
    {
        private MailMessage GetMailer()
        {
            var mail = new MailMessage
            {
                From = new MailAddress("rcemail1819@gmail.com")
            };
            return mail;
        }

        private SmtpClient GetSmtpClient()
        {
            var smtp = new SmtpClient
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("rcemail1819@gmail.com", "capstone1819"),
                Host = "smtp.gmail.com"
            };
            return smtp;
        }

        public bool SendTranscriptionDoneEmail(string email, Models.File file)
        {
            if (IsValid(email))
            {
                try
                {
                    var body = new StringBuilder();

                    var mail = GetMailer();
                    mail.To.Add(new MailAddress(email));
                    mail.IsBodyHtml = true;
                    mail.Subject = "Nouvelles Transcriptions";

                    body.AppendLine("<a href='http://localhost:59723/FileView/" + file.Id + "'>" + file.Title + "</a><br />");

                    mail.Body = "Liste de transcription: " + "<br />" + body.ToString();

                    var smtp = GetSmtpClient();
                    smtp.Send(mail);
                    smtp.Dispose();
                }
                catch
                {
                    return false;
                }
                return true;
            }

            else
                return false; 
        }

        public void SendReviewDoneEmail(string email, Models.File file, string reviewer)
        {
            var body = new StringBuilder();

            var mail = GetMailer();
            mail.To.Add(new MailAddress(email));
            mail.IsBodyHtml = true;
            mail.Subject = "Révision terminer pour fichier " + file.Title;

            body.AppendLine("<a href='http://localhost:59723/FileView/" + file.Id + "'>" + file.Title + "</a><br />");

            mail.Body = "Révision complètez par " + reviewer + "<br />" + "Cliquez sur ce lien pour accèder au fichier: " + "<br />" + body.ToString();

            var smtp = GetSmtpClient();
            smtp.Send(mail);
            smtp.Dispose();
        }

        public bool IsValid(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        }
    }
}
