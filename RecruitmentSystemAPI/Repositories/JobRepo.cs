using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class JobRepo
    {
        private readonly RecruitmentSystemContext _context;
        public JobRepo(RecruitmentSystemContext context)
        {
            _context = context;
        }

        public List<JobVM> GetCompanyJobsByUserId(string userId)
        {
            var jobs = _context.CompanyUsers.Where(cu => cu.UserId == userId).Include(cu => cu.Company).ThenInclude(c => c.Jobs).Select(c => c.Company.Jobs).FirstOrDefault();
            return jobs.Select(j => new JobVM
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                City = j.City,
                Province = j.Province,
                Country = j.Country,
                Address = j.Address,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                IsActive = j.IsActive,
                Sunday = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday = j.Weekdays.HasFlag(Weekdays.Saturday)
            }).ToList();
        }

        public List<JobVM> GetJobByCompanyId(int companyId)
        {
           // var jobs = _context.CompanyUsers.Where(cu => cu.UserId == userId).Include(cu => cu.Company).ThenInclude(c => c.Jobs).Select(c => c.Company.Jobs).FirstOrDefault();
           var jobs = _context.Jobs.Where(cId => cId.CompanyId == companyId).Include(cId => cId.Company).ThenInclude(c => c.Jobs).Select(c => c.Company.Jobs).FirstOrDefault();
            return jobs.Select(j => new JobVM
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                City = j.City,
                Province = j.Province,
                Country = j.Country,
                Address = j.Address,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                IsActive = j.IsActive,
                Sunday = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday = j.Weekdays.HasFlag(Weekdays.Saturday)
            }).ToList();
        }

        public JobVM GetJobById(int id)
        {
            return _context.Jobs.Where(j => j.Id == id).Select(j => new JobVM
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                City = j.City,
                Province = j.Province,
                Country = j.Country,
                Address = j.Address,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                IsActive = j.IsActive,
                Sunday = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday = j.Weekdays.HasFlag(Weekdays.Saturday)
            }).FirstOrDefault();
        }

        public void UpdateJob(JobVM jobVM)
        {
            var job = _context.Jobs.FirstOrDefault(c => c.Id == jobVM.Id);
            if (job == null) throw new KeyNotFoundException();

            job.Title = jobVM.Title;
            job.Description = jobVM.Description;
            job.City = jobVM.City;
            job.Province = jobVM.Province;
            job.Country = jobVM.Country;
            job.Address = jobVM.Address;
            job.StartDate = jobVM.StartDate;
            job.EndDate = jobVM.EndDate;
            job.IsActive = jobVM.IsActive;
            job.Weekdays = ConvertJobWeekdaysToEnum(jobVM);

            _context.Update(job);
            _context.SaveChanges();
        }

        private Weekdays ConvertJobWeekdaysToEnum(JobVM jobVM)
        {
            Weekdays weekdays = 0;
            if (jobVM.Sunday)
                weekdays |= Weekdays.Sunday;
            if (jobVM.Monday)
                weekdays |= Weekdays.Monday;
            if (jobVM.Tuesday)
                weekdays |= Weekdays.Tuesday;
            if (jobVM.Wednesday)
                weekdays |= Weekdays.Wednesday;
            if (jobVM.Thursday)
                weekdays |= Weekdays.Thursday;
            if (jobVM.Friday)
                weekdays |= Weekdays.Friday;
            if (jobVM.Saturday)
                weekdays |= Weekdays.Saturday;
            return weekdays;
        }

        public bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }

        public JobVM AddJob(JobVM jobVM, string userId)
        {
            var companyId = _context.CompanyUsers.FirstOrDefault(cu => cu.UserId == userId).CompanyId;
            var job = new Job
            {
                CompanyId = companyId,
                Title = jobVM.Title,
                Description = jobVM.Description,
                City = jobVM.City,
                Province = jobVM.Province,
                Country = jobVM.Country,
                Address = jobVM.Address,
                StartDate = jobVM.StartDate,
                EndDate = jobVM.EndDate,
                IsActive = jobVM.IsActive,
                Weekdays = ConvertJobWeekdaysToEnum(jobVM)
            };

            _context.Jobs.Add(job);
            _context.SaveChanges();
            return jobVM;
        }
    }
}
