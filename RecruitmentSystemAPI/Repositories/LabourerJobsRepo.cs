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

        public IQueryable<LabourerJobVM> GetLabourerJobsByUserRole(ClaimsPrincipal user, int count, int page, int? jobId, int? labourerId, int? companyId, out int totalRows, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var userId = _userManager.GetUserId(user);
            var query = _context.LabourerJobs
                .Where(l => (!fromDate.HasValue || l.Date >= fromDate) && (!toDate.HasValue || l.Date <= toDate))
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
            if (labourerId.HasValue)
            {
                query = query.Where(l => l.LabourerId == labourerId);
            }
            if (companyId.HasValue)
            {
                query = query.Where(l => l.Job.CompanyId == companyId);
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
                ChargeAmount = l.ChargeAmount,
                CompanyAddress = l.Job.Address,
                CompanyName = l.Job.Company.Name,
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
            var jobDate = jobToRate.Date;
            var today = DateTime.Today;
            var daysAfterJobDate = (today - jobDate).TotalDays;

            if (daysAfterJobDate > 14)
            {
                
                throw new Exception("You are not allowed to change the rating after 14 days");
            }

            else {

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
                UpdateLabourerRating(labourerJob.LabourerId);
            }
            else
            {
                throw new Exception($"Labourer job {labourerJobId} is not found");
            }
        }

        public (int, IEnumerable<InvoiceVM>) GetInvoices(DateTime fromDate, DateTime toDate, int? companyId, int count, int page)
        {
            var query = _context.LabourerJobs.Where(l => l.Date.Date >= fromDate && l.Date.Date <= toDate.Date && l.QualityRating.HasValue).Include(l => l.Job).ThenInclude(j => j.Company).Where(l => !companyId.HasValue || l.Job.CompanyId == companyId.Value)
                .OrderBy(l => l.Date).GroupBy(l => l.Job.Company);
            var totalRows = query.Count();
            var result = query.Skip(count * (page - 1)).Take(count).ToDictionary(l => l.Key, l => l.ToList()).Select(l => new InvoiceVM
            {
                CompanyId = l.Key.Id,
                CompanyEmail = l.Key.Email,
                CompanyName = l.Key.Name,
                CompanyPhone = l.Key.Phone,
                TotalToInvoice = l.Value.Sum(a => a.ChargeAmount * 8)
            });
            return (totalRows, result);
        }

        public (int, IEnumerable<InvoiceDetailsVM>) GetCompanyInvoiceDetails(int companyId, DateTime fromDate, DateTime toDate, int count, int page)
        {
            var query = _context.LabourerJobs.Where(l => l.Date.Date >= fromDate && l.Date.Date <= toDate.Date && l.QualityRating.HasValue).Include(l => l.Labourer).ThenInclude(l => l.User).Include(l => l.Skill).Include(l => l.Job)
                .Where(l => l.Job.CompanyId == companyId).OrderBy(l=>l.Date);
            var totalRows = query.Count();
            var result = query.Skip(count * (page - 1)).Take(count).Select(l => new InvoiceDetailsVM
            {
                ChargeAmount = l.ChargeAmount,
                Date = l.Date,
                JobTitle = l.Job.Title,
                LabourerEmail = l.Labourer.User.Email,
                LabourerName = $"{l.Labourer.FirstName} {l.Labourer.LastName}",
                LabourerPhone = l.Labourer.Phone,
                SkillName = l.Skill.Name
            });
            return (totalRows, result);
        }

        private void UpdateLabourerRating(int labourerId)
        {
            var avgQualityRating = _context.LabourerJobs.Where(lj => lj.LabourerId == labourerId && lj.QualityRating.HasValue).Select(lj=>lj.QualityRating.Value).ToList().DefaultIfEmpty().Average();
            var avgSafetyRating = _context.LabourerJobs.Where(lj => lj.LabourerId == labourerId && lj.SafetyRating.HasValue).Select(lj => lj.SafetyRating.Value).ToList().DefaultIfEmpty().Average();
            var labourer = _context.Labourers.FirstOrDefault(l => l.Id == labourerId);
            labourer.QualityRating = (float)avgQualityRating;
            labourer.SafetyRating = (float)avgSafetyRating;
            _context.Update(labourer);
            _context.SaveChanges();
        }
        public IEnumerable<LabourerJobReportVM> GetLabourerJobReport(ClaimsPrincipal user, int count, int page, int? labourerId, out int totalRows, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.LabourerJobs
                .Where(l => (!fromDate.HasValue || l.Date >= fromDate) && (!toDate.HasValue || l.Date <= toDate) && (!labourerId.HasValue || l.LabourerId == labourerId))
                .Include(l => l.Labourer).Include(l => l.Job).GroupBy(l => l.Labourer).AsQueryable();
            totalRows = query.Count();
            return query.Skip(count * (page - 1)).Take(count).ToDictionary(l => l.Key, l => l.ToList()).Select(l => new LabourerJobReportVM
            {
                LabourerId = l.Key.Id,
                LabourerFullName = $"{l.Key.FirstName} {l.Key.LastName}",
                TotalWage = l.Value.Sum(j=>j.WageAmount * 8),
                Jobs = l.Value.GroupBy(j => j.Job).Select(j => new BaseJobsVM
                {
                    WageAmount = j.Sum(a => a.WageAmount * 8),
                    JobId = j.Key.Id,
                    JobTitle = j.Key.Title
                }).ToList()
            });
        }
    }
}