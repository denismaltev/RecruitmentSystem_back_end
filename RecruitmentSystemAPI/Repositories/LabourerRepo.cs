using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class LabourerRepo
    {
        RecruitmentSystemContext _context;
        public LabourerRepo(RecruitmentSystemContext context)
        {
            _context = context;
        }

        public int? GetUserLabourerId(string userId)
        {
            return _context.Labourers.Where(l => l.UserId == userId).FirstOrDefault()?.Id;
        }
    }
}
