using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class LabourerSkillJob
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int LabourerSkillId { get; set; }
        public int? SafetyRating { get; set; }
        public int? QualityRating { get; set; }
        public DateTime Date { get; set; }
        public int? JobRating { get; set; }

        public virtual Job Job { get; set; }
        public virtual LabourerSkill LabourerSkill { get; set; }
    }
}
