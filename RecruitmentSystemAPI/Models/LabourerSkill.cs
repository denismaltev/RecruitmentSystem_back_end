using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class LabourerSkill
    {
        public LabourerSkill()
        {
            LabourerSkillJobs = new HashSet<LabourerSkillJob>();
        }

        public int Id { get; set; }
        public int LabourerId { get; set; }
        public int SkillId { get; set; }
        public bool IsActive { get; set; }

        public virtual Labourer Labourer { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual ICollection<LabourerSkillJob> LabourerSkillJobs { get; set; }
    }
}
