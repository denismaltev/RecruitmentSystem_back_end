using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using RecruitmentSystemAPI.Helpers;
using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Services
{
    public class ScheduledJob : BackgroundService
    {
        private CrontabSchedule _crontabSchedule;
        private DateTime _nextRun;
        protected string _schedule => "00 00 * * 5"; //every Friday at 12am
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScheduledJob(IServiceScopeFactory scopeFactory)
        {
            _serviceScopeFactory = scopeFactory;
            _crontabSchedule = CrontabSchedule.Parse(_schedule);
            _nextRun = _crontabSchedule.GetNextOccurrence(DateTime.Now);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                if (DateTime.Now > _nextRun)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetService<RecruitmentSystemContext>();
                        CreateSchedule(dbContext);
                    }
                    _nextRun = _crontabSchedule.GetNextOccurrence(DateTime.Now);
                }
                var delay = _nextRun - DateTimeOffset.Now;
                await Task.Delay(delay, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private void CreateSchedule(RecruitmentSystemContext dbContext)
        {
            var startDate = DateHelper.GetDayOfWeekDate(DateTime.Today.AddDays(7), DayOfWeek.Monday);
            var endDate = startDate.AddDays(6);
            var jobs = dbContext.Jobs
                .Where(j => j.IsActive && j.StartDate.Date <= endDate.Date && j.EndDate.Date >= startDate.Date)
                .Include(j => j.JobSkills).Include(j => j.LabourerJobs).Include(j=>j.Company)
                .OrderByDescending(j=>j.Company.Rating).ToList();
            if(jobs != null && jobs.Count > 0)
            {
                foreach(var job in jobs)
                {
                    AutoSchedule.MatchLabourersByDates(job, dbContext, startDate, endDate);
                }
            }
        }
    }
}

