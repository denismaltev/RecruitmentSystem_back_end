using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.Services;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class JobRepo : BaseRepo
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public JobRepo(RecruitmentSystemContext context, IServiceScopeFactory serviceScopeFactory) : base(context)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        // This is only for admin:
        public IQueryable<JobRatingVM> GetAllCompanyJobs()
        {
            return _context.Jobs.Select(j => new JobRatingVM
            {
                CompanyId   = j.CompanyId,
                CompanyName = j.Company.Name,
                Title       = j.Title,
                Rating      = j.Rating
            });
        }

        public List<JobVM> GetCompanyJobsByUserId(string userId)
        {
            var jobs = _context.CompanyUsers
                .Where(cu => cu.UserId == userId)
                .Include(cu => cu.Company)
                .ThenInclude(c => c.Jobs)
                .Select(c => c.Company.Jobs)
                .FirstOrDefault();
            //if(jobs == null) 
            return jobs.Select(j => new JobVM
            {
                Id = j.Id,
                CompanyId = j.CompanyId,
                Title = j.Title,
                Description = j.Description,
                City = j.City,
                Province = j.Province,
                Country = j.Country,
                Address = j.Address,
                Rating = j.Rating,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                Sunday = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday = j.Weekdays.HasFlag(Weekdays.Saturday),
                IsActive = j.IsActive
            }).ToList();
        }

        public List<JobVM> GetJobsByCompanyId(int companyId, int count, int page, out int totalRows, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var jobs = _context.Jobs
                  .Where(cId => cId.CompanyId == companyId)
                  .Where(cId => (!fromDate.HasValue || cId.StartDate >= fromDate) 
                  && (!toDate.HasValue || cId.EndDate < toDate)).AsQueryable();

            totalRows = jobs.Count();
            return jobs.OrderByDescending(cId => cId.StartDate).Skip(count * (page - 1)).Take(count)
                  .Select(j => new JobVM
                  {
                      Id = j.Id,
                      Title = j.Title,
                      Description = j.Description,
                      City = j.City,
                      Province = j.Province,
                      Country = j.Country,
                      Address = j.Address,
                      Rating = j.Rating,
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
            List<JobSkillVM> skillsList = _context.JobSkills.Where(js => js.JobId == id).Select(js => new JobSkillVM
            {
                Id = js.Skill.Id,
                Name = js.Skill.Name,
                NumberOfLabourersNeeded = js.NumberOfLabourersNeeded,
                IsActive = js.Skill.IsActive
            }).ToList();

            return _context.Jobs.Where(j => j.Id == id).Select(j => new JobVM
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                City = j.City,
                Province = j.Province,
                Country = j.Country,
                Address = j.Address,
                Rating = j.Rating,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                IsActive = j.IsActive,
                Sunday = j.Weekdays.HasFlag(Weekdays.Sunday),
                Monday = j.Weekdays.HasFlag(Weekdays.Monday),
                Tuesday = j.Weekdays.HasFlag(Weekdays.Tuesday),
                Wednesday = j.Weekdays.HasFlag(Weekdays.Wednesday),
                Thursday = j.Weekdays.HasFlag(Weekdays.Thursday),
                Friday = j.Weekdays.HasFlag(Weekdays.Friday),
                Saturday = j.Weekdays.HasFlag(Weekdays.Saturday),
                JobSkills = skillsList
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

            var jobSkills = _context.JobSkills.Where(js => js.JobId == jobVM.Id).ToList();
            if (jobSkills != null && jobSkills.Count > 0)
            {
                var skillsToDelete = jobSkills.Where(s => !jobVM.JobSkills.Any(ls => ls.Id == s.SkillId)).ToList();
                if (skillsToDelete != null && skillsToDelete.Count > 0)
                {
                    if (skillsToDelete != null && skillsToDelete.Count > 0)
                    {
                        _context.JobSkills.RemoveRange(skillsToDelete);
                    }
                }

                var skillsToUpdate = jobSkills.Where(s => jobVM.JobSkills.Any(ls => ls.Id == s.SkillId)).ToList();
                if (skillsToUpdate != null && skillsToUpdate.Count > 0)
                {
                    foreach (var skill in skillsToUpdate)
                    {
                        skill.NumberOfLabourersNeeded = jobVM.JobSkills.FirstOrDefault(s => s.Id == skill.SkillId).NumberOfLabourersNeeded;
                        _context.Update(skill);
                    }
                }
            }

            var newSkills = jobVM.JobSkills.Where(s => !jobSkills.Any(ls => ls.SkillId == s.Id)).ToList();
            if (newSkills != null && newSkills.Count > 0)
            {
                foreach (var skill in newSkills)
                {
                    var newSkill = new JobSkill
                    {
                        JobId = job.Id,
                        SkillId = skill.Id.Value,
                        NumberOfLabourersNeeded = skill.NumberOfLabourersNeeded
                    };
                    _context.Add(newSkill);
                }
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

            if (jobVM.JobSkills != null && jobVM.JobSkills.Count > 0)
            {
                foreach (var skill in jobVM.JobSkills)
                {
                    var newJobSkill = new JobSkill
                    {
                        Job = job,
                        SkillId = skill.Id.Value,
                        NumberOfLabourersNeeded = skill.NumberOfLabourersNeeded,
                    };
                    _context.JobSkills.Add(newJobSkill);
                }
            }
            _context.SaveChanges();
            new AutoSchedule(_serviceScopeFactory).MatchLabourers(job);
            return jobVM;
        }
    }
}
