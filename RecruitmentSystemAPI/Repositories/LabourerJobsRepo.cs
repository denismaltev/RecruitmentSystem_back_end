using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class LabourerJobsRepo
    {
        private readonly RecruitmentSystemContext _context;
        public LabourerJobsRepo(RecruitmentSystemContext context)
        {
            _context = context;
        }

        public IQueryable<LabourerJobVM> GetLabourerJobsByUserId(string userId, int count, int page, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return _context.LabourerSkillJobs
                .Where(l => (!fromDate.HasValue || l.Date >= fromDate) && (!toDate.HasValue || l.Date < toDate))
                .Include(l => l.LabourerSkill).ThenInclude(ls => ls.Skill).Include(l => l.LabourerSkill.Labourer).Where(l => l.LabourerSkill.Labourer.UserId == userId)
                .Include(l => l.Job).OrderByDescending(l => l.Date).Skip(count * (page - 1)).Take(count).Select(l => new LabourerJobVM
                {
                    Id = l.Id,
                    JobTitle = l.Job.Title,
                    SkillName = l.LabourerSkill.Skill.Name,
                    Date = l.Date,
                    SafetyRating = l.SafetyRating,
                    QualityRating = l.QualityRating,
                    JobRating = l.JobRating,
                    WageAmount = l.WageAmount
                });
        }

        public LabourerJobVM AddLabourerSkillJob(LabourerJobVM labourerJobVM, string userId)
        {
            var labourerSkill = _context.LabourerSkills.Where(ls => ls.SkillId == labourerJobVM.SkillId.Value).Include(ls => ls.Labourer).Where(ls => ls.Labourer.UserId == userId).FirstOrDefault();
            var labourerSkillJob = new LabourerSkillJob
            {
                JobId = labourerJobVM.JobId.Value,
                LabourerSkill = labourerSkill,
                Date = labourerJobVM.Date,
                JobRating = labourerJobVM.JobRating,
                QualityRating = labourerJobVM.QualityRating,
                SafetyRating = labourerJobVM.SafetyRating,
                WageAmount = labourerJobVM.WageAmount
            };
            _context.LabourerSkillJobs.Add(labourerSkillJob);
            _context.SaveChanges();
            labourerJobVM.Id = labourerSkillJob.Id;
            return labourerJobVM;
        }
    }
}
