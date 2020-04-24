using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class LabourerJobVM
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public int? JobId { get; set; }
        public string JobTitle { get; set; }
        public int? SkillId { get; set; }
        public string SkillName { get; set; }
        public DateTime Date { get; set; }
        public int? SafetyRating { get; set; }
        public int? QualityRating { get; set; }
        public int? JobRating { get; set; }
        public decimal WageAmount { get; set; }
    }
}
