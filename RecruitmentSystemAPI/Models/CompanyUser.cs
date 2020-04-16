using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class CompanyUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual SystemUser User { get; set; }
    }
}
