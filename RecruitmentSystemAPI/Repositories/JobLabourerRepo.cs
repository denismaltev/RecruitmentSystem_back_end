using Microsoft.Extensions.DependencyInjection;
using RecruitmentSystemAPI.Models;
using RecruitmentSystemAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class JobLabourerRepo : BaseRepo
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JobLabourerRepo(RecruitmentSystemContext context, IServiceScopeFactory serviceScopeFactory) : base(context)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IQueryable<JobLabourerVM> GetLabourersList(int jobId)
        {
            // first name + last name + phone on Labourer table where labourer.id == labourerJobs.labourerId and labourerJobs.JobId == Jobs.Id 
            //and
            //select skills.Name where skills.id == labourerjobs.skillId and jobId == jobs.id 
            var fullName = _context.LabourerJobs
                .Where(l => l.JobId == jobId)
                .Where(l => l.LabourerId == l.Labourer.Id)
                .Select(l => new { l.Labourer.FirstName, l.Labourer.LastName }).FirstOrDefault().ToString();

            var phoneNumber = _context.LabourerJobs
                .Where(l => l.JobId == jobId)
                .Where(l => l.LabourerId == l.Labourer.Id)
                .Select(l => l.Labourer.Phone).FirstOrDefault();

            var skillName = _context.LabourerJobs
                .Where(l => l.JobId == jobId)
                .Where(l => l.LabourerId == l.Labourer.Id)
                .Where(l => l.SkillId == l.Skill.Id)
                .Select(s => s.Skill.Name).FirstOrDefault();

            //.Where(j => j.Id == _context.LabourerJobs.)

            return _context.Jobs.Select(j => new JobLabourerVM
            {
                JobId = j.Id,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                SkillName = skillName
            }); 
        }
    }
}
