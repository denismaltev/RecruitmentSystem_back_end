using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Helpers
{
    public class EmailHelper
    {
        private EmailSettings emailSettings;
        public EmailHelper(EmailSettings emailSettings)
        {

            this.emailSettings = emailSettings;
        }

        public void BuildEmailBodyForLabourer(RecruitmentSystemContext dbContext, Job job, List<LabourerJob> labourerJobs)
        {
            var mailingListOfLabourers = dbContext.Labourers
                .Where(l => l.LabourerJobs.Any(lj => lj.LabourerId == l.Id))
                .Include(l => l.User)
                .Select(l => new
                {
                    id = l.Id,
                    email = l.User.Email,
                    name = $"{ l.FirstName} {l.LastName}"
                });

            if (mailingListOfLabourers != null)
            {
                EmailSender emailSender = new EmailSender(emailSettings);
                string subject = "[RecruitmentSystem]: You have been assigned to a new job.";
                foreach (var laborer in mailingListOfLabourers)
                {

                    string text = $"Dear {laborer.name} You have been assigned to a new job.";
                    string html = @"<p>Dear " + laborer.name + "</p><p> You have been assigned to a new job.</p>" +
                                   "<hr /><p> Details </p><hr /><p> Company: " + job.Title + "</p>" +
                                   "<p> Job description: " + job.Description + "</p><p> Location: " +
                                   job.Country + " " + job.Province + " " + job.City + " " + job.Address + "</p>" +
                                   "<hr /><p> Your schedule:</p><hr /><p>" + getJobScheduleForLaborerByLaborerId(labourerJobs, laborer.id) + "</p><p> Congratulations! </p>";

                    emailSender.SendMail(laborer.email, subject, text, html);
                    //SendMail("test08081979@gmail.com", subject, text, html);  // TESTING
                }
            }
        }

        public String getJobScheduleForLaborerByLaborerId(List<LabourerJob> labourerJobs, int laborerId)
        {
            if (labourerJobs != null)
            {
                String result = "";
                foreach (var labourerJob in labourerJobs)
                {
                    if (labourerJob.LabourerId == laborerId)
                    {
                        result += $"<p>{labourerJob.Date.ToString("dddd, dd MMMM yyyy")}<p>";
                    }
                }
                return result;
            }
            return "";
        }
    }
}
