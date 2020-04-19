using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class Company
    {
        public Company()
        {
            CompanyUsers = new HashSet<CompanyUser>();
            Jobs = new HashSet<Job>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }

        public virtual ICollection<CompanyUser> CompanyUsers { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
