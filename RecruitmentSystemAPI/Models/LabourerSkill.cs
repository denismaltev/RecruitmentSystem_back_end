using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class LabourerSkill
    {
        public LabourerSkill()
        {
        }

        public int Id { get; set; }
        public int LabourerId { get; set; }
        public int SkillId { get; set; }

        public virtual Labourer Labourer { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
