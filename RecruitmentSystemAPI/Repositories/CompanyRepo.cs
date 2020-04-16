using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class CompanyRepo
    {
        RecruitmentSystemContext _context;
        public CompanyRepo(RecruitmentSystemContext context)
        {
            _context = context;
        }

        public int? GetUserCompanyId(string userId)
        {
            return _context.CompanyUsers.Where(cu => cu.UserId == userId).FirstOrDefault()?.CompanyId;
        }
    }
}
