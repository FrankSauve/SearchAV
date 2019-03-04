using System;
using System.Net;
using System.Net.Mail;
using System.Text;

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

        public void SendTranscriptionDoneEmail(string email, Models.File file)
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

        public void SendReviewDoneEmail(string email, Models.File file, string reviewer)
        {
            var body = new StringBuilder();

            var mail = GetMailer();
            mail.To.Add(new MailAddress(email));
            mail.IsBodyHtml = true;
            mail.Subject = "Révision Terminez pour fichier " + file.Title;

            body.AppendLine("<a href='http://localhost:59723/FileView/" + file.Id + "'>" + file.Title + "</a><br />");

            mail.Body = "Révision complètez par " + reviewer + "<br />" + "Lien pour révision finale: " + "<br />" + body.ToString();

            var smtp = GetSmtpClient();
            smtp.Send(mail);
            smtp.Dispose();
        }
    }
}
