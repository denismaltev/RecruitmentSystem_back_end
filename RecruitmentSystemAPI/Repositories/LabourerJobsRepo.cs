using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class LabourerJobsRepo : BaseRepo
    {
        private readonly UserManager<SystemUser> _userManager;
        public LabourerJobsRepo(RecruitmentSystemContext context, UserManager<SystemUser> userManager) : base(context)
        {
            _userManager = userManager;
        }
        public IQueryable<JobLabourerVM> GetLabourersList(int jobId)
        {
            return _context.LabourerJobs
               .Where(l => l.JobId == jobId)
               .Include(l => l.Labourer)
               .Include(l => l.Skill)
               .Where(l => l.LabourerId == l.Labourer.Id)
               .Select(l => new JobLabourerVM { 
                   JobId        = l.JobId, 
                   FullName     = l.Labourer.FirstName + " " + l.Labourer.LastName, 
                   PhoneNumber  = l.Labourer.Phone, SkillName = l.Skill.Name });
        }

        public IQueryable<LabourerJobVM> GetLabourerJobsByUserRole(ClaimsPrincipal user, int count, int page, int? jobId, int? labourerId, out int totalRows, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var userId = _userManager.GetUserId(user);
            var query = _context.LabourerJobs
                .Where(l => (!fromDate.HasValue || l.Date >= fromDate) && (!toDate.HasValue || l.Date < toDate))
                .Include(l => l.Skill).Include(l => l.Labourer)
                .Include(l => l.Job).ThenInclude(l => l.Company).AsQueryable();
            if (user.IsInRole("Labourer"))
            {
                query = query.Where(l => l.Labourer.UserId == userId);
            }
            else if (user.IsInRole("Company"))
            {
                query = query.Where(l => _context.CompanyUsers.FirstOrDefault(cu => cu.UserId == userId).CompanyId == l.Job.CompanyId);
            }


            if (jobId.HasValue)
            {
                query = query.Where(l => l.JobId == jobId.Value);
            }

            else if (labourerId.HasValue)
            {
                query = query.Where(l => l.LabourerId == labourerId);
            }


            totalRows = query.Count();

            return query.OrderByDescending(l => l.Date).Skip(count * (page - 1)).Take(count).Select(l => new LabourerJobVM
            {
                Id = l.Id,
                JobTitle = l.Job.Title,
                SkillName = l.Skill.Name,
                Date = l.Date,
                SafetyRating = l.SafetyRating,
                QualityRating = l.QualityRating,
                JobRating = l.JobRating,
                WageAmount = l.WageAmount,
                CompanyAddress = l.Job.Company.Name,
                CompanyName = l.Job.Address,
                LabourerFullName = $"{l.Labourer.FirstName} {l.Labourer.LastName}",
                LabourerPhone = l.Labourer.Phone
            });
        }
        
        public LabourerJobVM AddLabourerJob(LabourerJobVM labourerJobVM, string userId)
        {
            var labourerSkill = _context.LabourerSkills.Where(ls => ls.SkillId == labourerJobVM.SkillId.Value).Include(ls => ls.Labourer).Where(ls => ls.Labourer.UserId == userId).FirstOrDefault();
            var labourerJob = new LabourerJob
            {
                JobId = labourerJobVM.JobId.Value,
                SkillId = labourerSkill.SkillId,
                Labourer = labourerSkill.Labourer,
                Date = labourerJobVM.Date,
                JobRating = labourerJobVM.JobRating,
                QualityRating = labourerJobVM.QualityRating,
                SafetyRating = labourerJobVM.SafetyRating,
                WageAmount = labourerJobVM.WageAmount
            };
            _context.LabourerJobs.Add(labourerJob);
            _context.SaveChanges();
            labourerJobVM.Id = labourerJob.Id;
            return labourerJobVM;
        }

        public void UpdateJobRating(int idToGrade, int rating, string userId)
        {
            var jobToRate = _context.LabourerJobs.Where(ls => ls.Id == idToGrade && ls.Labourer.UserId == userId).FirstOrDefault();

            if (jobToRate.JobRating ==null){

                //update rating in labourerjobs table
                jobToRate.JobRating = rating;
                _context.Update(jobToRate);
                _context.SaveChanges();

                //update rating in jobs table
                var jobId = jobToRate.JobId;
                var jobInTable = _context.Jobs.Where(j => j.Id == jobId).FirstOrDefault();
                var previousJobRate = jobInTable.Rating;
                var allJobWithSameId = _context.LabourerJobs.Where(ls => ls.JobId == jobId).Select(j => j.JobRating).Where(v => v != 0 && v != null).ToArray();
                var newJobRating = (float)allJobWithSameId.Average();
                jobInTable.Rating = newJobRating;
                _context.Update(jobInTable);
                _context.SaveChanges();

                //update rating in companies table
                var CompanyToRate = _context.Companies.Where(c => c.Id == jobInTable.CompanyId).FirstOrDefault();
                var allJobWithSameCompanyId = _context.Jobs.Where(j => j.CompanyId == jobInTable.CompanyId).Select(j => j.Rating).Where(v => v!= 0 && v != null).ToArray();
                var newCompanyRating = (float)allJobWithSameCompanyId.Average();
                CompanyToRate.Rating = newCompanyRating;
                _context.Update(CompanyToRate);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("You have already graded this job");
            }
        
        }

        public void UpdateLabourerJob(string userId, int labourerJobId, bool isAdmin, int? qualityRating, int? safetyRating)
        {
            var query = _context.LabourerJobs.Where(l => l.Id == labourerJobId);
            if (!isAdmin)
            {
                query = query.Include(l => l.Job).ThenInclude(j => j.Company).ThenInclude(c => c.CompanyUsers).Where(l => l.Job.Company.CompanyUsers.Any(u => u.UserId == userId));
            }
            var labourerJob = query.FirstOrDefault();
            if (labourerJob != null)
            {
                if (qualityRating.HasValue)
                {
                    labourerJob.QualityRating = qualityRating.Value;
                }
                if (safetyRating.HasValue)
                {
                    labourerJob.SafetyRating = safetyRating.Value;
                }
                _context.Update(labourerJob);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"Labourer job {labourerJobId} is not found");
            }
        }
    }
}