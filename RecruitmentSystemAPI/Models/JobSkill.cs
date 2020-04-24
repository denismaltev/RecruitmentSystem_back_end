using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class JobSkill
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int SkillId { get; set; }
        public int NumberOfLabourersNeeded { get; set; }

        public virtual Job Job { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
