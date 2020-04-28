using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Services
{
    public class EmailSender
    {
        private EmailSettings emailSettings;
        public  EmailSender (EmailSettings emailSettings)
        {
            this.emailSettings = emailSettings;
        }

        public bool SendMail (string recipient, string subject)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(emailSettings.FromEmail, emailSettings.DisplayName)
                };
                string toEmail = string.IsNullOrEmpty(recipient) ? emailSettings.ToEmail : recipient;
                mail.To.Add(new MailAddress(toEmail));
                foreach(string bcc in emailSettings.BccEmails)
                {
                    mail.Bcc.Add(new MailAddress(bcc));
                }

                mail.Subject = subject;
                string text = "Plain text version of a message body. ";
                string html = @"<p>HTML version of a message body. </p>";

                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                mail.Priority = MailPriority.High;

                SmtpClient smtp = new SmtpClient(emailSettings.Domain, emailSettings.Port);
                smtp.Credentials = new NetworkCredential(emailSettings.UsernameLogin, emailSettings.UsernamePassword);
                smtp.EnableSsl = false;
                smtp.Send(mail);
            } catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
