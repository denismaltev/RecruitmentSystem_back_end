using Microsoft.AspNetCore.Identity;
using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Repositories
{
    public class SystemUserRepo : BaseRepo
    {
        public SystemUserRepo(RecruitmentSystemContext context) : base(context)
        {
        }

        public string GetUserRoleName(string userId)
        {
            return _context.Roles.FirstOrDefault(r => r.Id == _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId).RoleId)?.Name;
        }

        public IdentityRole GetRoleByName(string roleName)
        {
            return _context.Roles.FirstOrDefault(r => r.Name.ToLower() == roleName.ToLower());
        }
    }
}
