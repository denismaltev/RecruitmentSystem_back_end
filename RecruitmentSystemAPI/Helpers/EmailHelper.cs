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

        public void BuildEmailBodyForCompany(RecruitmentSystemContext dbContext, Job job, List<LabourerJob> labourerJobs)
        {
            var company = dbContext.Companies.FirstOrDefault(c => c.Id == job.CompanyId);
            string scheduleOfAssignedLabourers = "";
            string subject = "[RecruitmentSystem]: The schedule for your job was created.";
            List<Contact> labourersContactList = GetLabourersContactList(dbContext);
            Contact contact = new Contact();
            if (labourersContactList != null)
            {
                EmailSender emailSender = new EmailSender(emailSettings);
                foreach (var lj in labourerJobs)
                {
                    scheduleOfAssignedLabourers += $"<p> {lj.Date.ToString("dddd, dd MMMM yyyy")} ";
                    contact = labourersContactList.Find(lc => lc.id == lj.LabourerId);
                    //contact != null ? scheduleOfAssignedLabourers += $"{contact.name} </p>" : scheduleOfAssignedLabourers += " N/A </p>";
                    if (contact != null) {
                        scheduleOfAssignedLabourers += $"{contact.name} </p>";
                    } else {
                        scheduleOfAssignedLabourers += " N/A </p>";
                    } 
                }
                string text = $"Dear {company.Name}. The schedule for your job {job.Title} was created.";
                string html = @"<p>Dear " + company.Name + ".</p><p> The schedule for your job " + job.Title + " was created.</p>" +
                              "<p>Start Date: " + job.StartDate.ToString("dddd, dd MMMM yyyy") +
                              "  End Date: " + job.EndDate.ToString("dddd, dd MMMM yyyy") + "</p>" +
                             "<p> Schedule:</p><hr />" + scheduleOfAssignedLabourers + "<p> Congratulations! </p>";
                emailSender.SendMail(company.Email, subject, text, html);
                //emailSender.SendMail("test08081979@gmail.com", subject, text, html);  // TESTING
            } 
        }

        public void BuildEmailBodyForLabourer(RecruitmentSystemContext dbContext, Job job, List<LabourerJob> labourerJobs)
        {

            var labourersContactList = GetLabourersContactList(dbContext);

            if (labourersContactList != null)
            {
                EmailSender emailSender = new EmailSender(emailSettings);
                string subject = "[RecruitmentSystem]: You have been assigned to a new job.";
                foreach (var laborer in labourersContactList)
                {

                    string text = $"Dear {laborer.name}. You have been assigned to a new job.";
                    string html = @"<p>Dear " + laborer.name + ".</p><p> You have been assigned to a new job.</p>" +
                                   "<hr /><p> Details </p><hr /><p> Company: " + job.Title + "</p>" +
                                   "<p> Job description: " + job.Description + "</p><p> Location: " +
                                   job.Country + " " + job.Province + " " + job.City + " " + job.Address + "</p>" +
                                   "<hr /><p> Your schedule:</p><hr /><p>" + getJobScheduleForLaborerByLaborerId(labourerJobs, laborer.id) +
                                   "</p><p> Congratulations! </p>";

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

        public List<Contact> GetLabourersContactList(RecruitmentSystemContext dbContext)
        {
            return dbContext.Labourers
                .Where(l => l.LabourerJobs.Any(lj => lj.LabourerId == l.Id))
                .Include(l => l.User)
                .Select(l => new Contact
                {
                    id = l.Id,
                    email = l.User.Email,
                    name = $"{ l.FirstName} {l.LastName}"
                }).ToList();
        }
    }
}
