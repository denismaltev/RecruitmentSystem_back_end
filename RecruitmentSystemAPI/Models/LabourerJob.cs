using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class LabourerJob
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int LabourerId { get; set; }
        public int SkillId { get; set; }
        public int? SafetyRating { get; set; }
        public int? QualityRating { get; set; }
        public DateTime Date { get; set; }
        public int? JobRating { get; set; }
        public decimal WageAmount { get; set; }

        public virtual Job Job { get; set; }
        public virtual Labourer Labourer { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
