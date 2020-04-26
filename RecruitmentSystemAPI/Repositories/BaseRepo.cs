using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public abstract class BaseRepo
    {
        protected readonly RecruitmentSystemContext _context;
        public BaseRepo(RecruitmentSystemContext context)
        {
            _context = context;
        }
    }
}
