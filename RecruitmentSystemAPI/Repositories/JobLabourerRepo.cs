using Microsoft.EntityFrameworkCore;
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
            return _context.LabourerJobs
               .Where(l => l.JobId == jobId)
               .Include(l => l.Labourer)
               .Include(l => l.Skill)
               .Where(l => l.LabourerId == l.Labourer.Id)
               .Select(l => new JobLabourerVM { JobId = l.JobId, FullName = l.Labourer.FirstName + " " + l.Labourer.LastName, PhoneNumber = l.Labourer.Phone, SkillName = l.Skill.Name });
        }
    }
}
