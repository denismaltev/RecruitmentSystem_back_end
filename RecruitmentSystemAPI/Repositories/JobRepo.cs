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

        // This is only for admin:
        public IQueryable<JobVM> GetAllCompanyJobs()
        {
            return _context.Jobs.Select(j => new JobVM
            {
                Id          = j.Id,
                CompanyId   = j.CompanyId,
                Title       = j.Title,
                Description = j.Description,
                City        = j.City,
                Province    = j.Province,
                Country     = j.Country,
                Address     = j.Address,
                Rating      = j.Rating,
                StartDate   = j.StartDate,
                EndDate     = j.EndDate,
                Sunday      = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday      = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday     = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday   = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday    = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday      = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday    = j.Weekdays.HasFlag(Weekdays.Saturday),
                IsActive    = j.IsActive
            });
        }

        public List<JobVM> GetCompanyJobsByUserId(string userId)
        {
            var jobs = _context.CompanyUsers.Where(cu => cu.UserId == userId).Include(cu => cu.Company).ThenInclude(c => c.Jobs).Select(c => c.Company.Jobs).FirstOrDefault();
            //if(jobs == null) 
            return jobs.Select(j => new JobVM
            {
                Id          = j.Id,
                CompanyId   = j.CompanyId,
                Title       = j.Title,
                Description = j.Description,
                City        = j.City,
                Province    = j.Province,
                Country     = j.Country,
                Address     = j.Address,
                Rating      = j.Rating,
                StartDate   = j.StartDate,
                EndDate     = j.EndDate,
                Sunday      = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday      = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday     = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday   = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday    = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday      = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday    = j.Weekdays.HasFlag(Weekdays.Saturday),
                IsActive    = j.IsActive
            }).ToList();
        }

        public List<JobVM> GetJobsByCompanyId(int companyId, int count, int page, DateTime? fromDate = null, DateTime? toDate = null)
        {
           var jobs = _context.Jobs
                 .Where(cId => cId.CompanyId == companyId)
                 .Where(cId => (!fromDate.HasValue || cId.StartDate >= fromDate) && (!toDate.HasValue || cId.EndDate < toDate))
                 .OrderByDescending(cId => cId.StartDate).Skip(count * (page - 1)).Take(count)
                 .Select(j =>  new JobVM
            {
                Id          = j.Id,
                Title       = j.Title,
                Description = j.Description,
                City        = j.City,
                Province    = j.Province,
                Country     = j.Country,
                Address     = j.Address,
                Rating      = j.Rating,
                StartDate   = j.StartDate,
                EndDate     = j.EndDate,
                IsActive    = j.IsActive,
                Sunday      = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday      = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday     = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday   = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday    = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday      = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday    = j.Weekdays.HasFlag(Weekdays.Saturday)
            }).ToList();
            return jobs;
        }

        public JobVM GetJobById(int id)
        {
            List<JobSkillVM> skillsList = _context.JobSkills.Where(js => js.JobId == id).Select(js => new JobSkillVM
            {
                SkillId                 = js.Skill.Id,
                SkillName               = js.Skill.Name,
                NumberOfLabourersNeeded = js.NumberOfLabourersNeeded,
                IsActive                = js.Skill.IsActive
            }).ToList();

            return _context.Jobs.Where(j => j.Id == id).Select(j => new JobVM
            {
                Id          = j.Id,
                Title       = j.Title,
                Description = j.Description,
                City        = j.City,
                Province    = j.Province,
                Country     = j.Country,
                Address     = j.Address,
                Rating      = j.Rating,
                StartDate   = j.StartDate,
                EndDate     = j.EndDate,
                IsActive    = j.IsActive,
                Sunday      = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday      = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday     = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday   = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday    = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday      = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday    = j.Weekdays.HasFlag(Weekdays.Saturday),
                JobSkills   = skillsList
            }).FirstOrDefault();
        }

        public void UpdateJob(JobVM jobVM)
        {
            var job = _context.Jobs.FirstOrDefault(c => c.Id == jobVM.Id);
            if (job == null) throw new KeyNotFoundException();

            job.Title       = jobVM.Title;
            job.Description = jobVM.Description;
            job.City        = jobVM.City;
            job.Province    = jobVM.Province;
            job.Country     = jobVM.Country;
            job.Address     = jobVM.Address;
            job.StartDate   = jobVM.StartDate;
            job.EndDate     = jobVM.EndDate;
            job.IsActive    = jobVM.IsActive;
            job.Weekdays    = ConvertJobWeekdaysToEnum(jobVM);

            // Delete old skills
            var jobSkills = _context.JobSkills.Where(js => js.JobId == jobVM.Id);
            foreach(var jobSkill in jobSkills)
            {
                _context.JobSkills.Remove(jobSkill);
            }
            // Add skills from client
            List<JobSkillVM> skillsFromClient = jobVM.JobSkills;
            foreach(var skillFromClient in skillsFromClient)
            {
                var newJobSkill = new JobSkill
                {
                    Job = job,
                    SkillId = skillFromClient.SkillId,
                    NumberOfLabourersNeeded = skillFromClient.NumberOfLabourersNeeded,
                    IsActive = skillFromClient.IsActive
                };
                _context.JobSkills.Add(newJobSkill);
            }

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
                CompanyId    = companyId,
                Title        = jobVM.Title,
                Description  = jobVM.Description,
                City         = jobVM.City,
                Province     = jobVM.Province,
                Country      = jobVM.Country,
                Address      = jobVM.Address,
                StartDate    = jobVM.StartDate,
                EndDate      = jobVM.EndDate,
                IsActive     = jobVM.IsActive,
                Weekdays     = ConvertJobWeekdaysToEnum(jobVM)
            };
            _context.Jobs.Add(job);

            List<JobSkillVM> Skills = jobVM.JobSkills;
            foreach(var skill in Skills)
            {
                var newJobSkill = new JobSkill
                {
                    Job = job,
                    SkillId = skill.SkillId,
                    NumberOfLabourersNeeded = skill.NumberOfLabourersNeeded,
                    IsActive = skill.IsActive
                };
                _context.JobSkills.Add(newJobSkill);
            }
            _context.SaveChanges();
            return jobVM;
        }
    }
}
