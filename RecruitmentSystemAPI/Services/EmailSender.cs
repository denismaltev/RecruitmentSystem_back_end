using Microsoft.EntityFrameworkCore;
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
        public EmailSender(EmailSettings emailSettings)
        {

            this.emailSettings = emailSettings;
        }

        public void SendMailToLabourers(RecruitmentSystemContext dbContext, Job job, List<LabourerJob> labourerJobs)
        {
            var mailingListOfLabourers = dbContext.Labourers
                .Where(l => l.LabourerJobs.Any(lj => lj.LabourerId == l.Id))
                .Include(l => l.User)
                .Select(l => new
                {
                    email = l.User.Email,
                    name = $"{ l.FirstName} {l.LastName}"
                });

            if (mailingListOfLabourers != null)
            {
                string subject = "[RecruitmentSystem]: You have been assigned to a new job.";
                foreach (var laborer in mailingListOfLabourers)
                {

                    string text = $"Dear {laborer.name} You have been assigned to a new job.";
                    string html = @"<p>Dear " + laborer.name + "</p><p> You have been assigned to a new job.</p>" +
                                   "<hr /><p> Details </p><hr /><p> Company: " + job.Title + "</p>" +
                                   "<p> Job description: " + job.Description + "</p><p> Location: " +
                                   job.Country + job.Province + job.City + job.Address + "</p>" +
                                   "<hr /><p> Your schedule:</p><hr /><p></p><p> Congratulations! </p>";
                    //SendMail(laborer.email, textOfLetter);
                    //SendMail("test08081979@gmail.com", subject, text, html);  // TESTING
                }
            }
        }

        public bool SendMail(string recipient, string subject, string text, string html)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(emailSettings.FromEmail, emailSettings.DisplayName)
                };
                string toEmail = string.IsNullOrEmpty(recipient) ? emailSettings.ToEmail : recipient;
                mail.To.Add(new MailAddress(toEmail));
                if (emailSettings.BccEmails != null)
                {
                    foreach (string bcc in emailSettings.BccEmails)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                }
                mail.Subject = subject;

                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                mail.Priority = MailPriority.High;

                SmtpClient smtp = new SmtpClient(emailSettings.Domain, emailSettings.Port);
                smtp.Credentials = new NetworkCredential(emailSettings.UsernameLogin, emailSettings.UsernamePassword);
                smtp.EnableSsl = false;
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
