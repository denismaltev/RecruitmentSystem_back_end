using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class Skill
    {
        public Skill()
        {
            JobSkills = new HashSet<JobSkill>();
            LabourerSkills = new HashSet<LabourerSkill>();
            LabourerJobs = new HashSet<LabourerJob>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal PayAmount { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<JobSkill> JobSkills { get; set; }
        public virtual ICollection<LabourerSkill> LabourerSkills { get; set; }
        public virtual ICollection<LabourerJob> LabourerJobs { get; set; }
    }
}
