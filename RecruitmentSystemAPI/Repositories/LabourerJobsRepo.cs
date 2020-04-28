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

        public IQueryable<LabourerJobVM> GetLabourerJobsByUserRole(ClaimsPrincipal user, int count, int page, out int totalRows, DateTime? fromDate = null, DateTime? toDate = null)
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
    }
}