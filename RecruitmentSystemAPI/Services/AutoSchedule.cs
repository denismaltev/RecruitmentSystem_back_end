using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecruitmentSystemAPI.Helpers;
using Microsoft.Extensions.Options;

namespace RecruitmentSystemAPI.Services
{
    public class AutoSchedule
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public AutoSchedule(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void MatchLabourers(Job job)
        {
            Task.Run(() =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<RecruitmentSystemContext>();
                    var emailSettings = scope.ServiceProvider.GetService<IOptions<EmailSettings>>();
                    MatchLabourersForTheNearestTwoWeeks(job, dbContext, emailSettings);
                }
            });
        }

        private void MatchLabourersForTheNearestTwoWeeks(Job job, RecruitmentSystemContext dbContext, IOptions<EmailSettings> emailSettings)
        {
            var startDate = DateTime.Now.AddDays(1); //from tomorrow
            var nextSunday = DateHelper.GetDayOfWeekDate(DateTime.Today, DayOfWeek.Sunday);
            var endDate = nextSunday.AddDays(7);
            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday) //friday schedule is already done
            {
                endDate = endDate.AddDays(7);
            }
            if (job.StartDate.Date <= endDate && job.EndDate.Date > startDate)
            {
                MatchLabourersByDates(job, dbContext, startDate, endDate, emailSettings);
            }
        }

        public static void MatchLabourersByDates(Job job, RecruitmentSystemContext dbContext, DateTime startDate, DateTime endDate, IOptions<EmailSettings> emailSettings)
        {
            var labourerJobsList = new List<LabourerJob>();  // Variable to collect labourerJobs
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var weekday = (Weekdays)Enum.Parse(typeof(Weekdays), date.DayOfWeek.ToString());
                if (date >= job.StartDate && date <= job.EndDate && job.Weekdays.HasFlag(weekday))
                {
                    foreach (var jobSkill in job.JobSkills)
                    {
                        var labourers = dbContext.Labourers
                            .Where(l => l.IsActive && l.Availability.HasFlag(weekday) && !dbContext.LabourerJobs.Any(lj => lj.LabourerId == l.Id && lj.Date.Date == date))
                            .Include(l => l.LabourerSkills)
                            .Where(l => l.LabourerSkills.Any(s => s.SkillId == jobSkill.SkillId))
                            .OrderByDescending(l => l.QualityRating).OrderByDescending(l => l.SafetyRating).Take(jobSkill.NumberOfLabourersNeeded).ToList();
                        if (labourers != null && labourers.Count > 0)
                        {
                            var skill = dbContext.Skills.FirstOrDefault(s => s.Id == jobSkill.SkillId);
                            var labourerJobs = labourers.Select(l => new LabourerJob
                            {
                                Date = date,
                                JobId = job.Id,
                                LabourerId = l.Id,
                                SkillId = jobSkill.SkillId,
                                WageAmount = skill.PayAmount,
                                ChargeAmount = skill.ChargeAmount
                            });
                            dbContext.LabourerJobs.AddRange(labourerJobs);
                            dbContext.SaveChanges();
                            labourerJobsList.AddRange(labourerJobs.ToList());
                        }
                    }
                    
                }
            }
            // send emails
            new EmailHelper(emailSettings.Value).StartMailing(dbContext, job, labourerJobsList);
        }
    }
}
