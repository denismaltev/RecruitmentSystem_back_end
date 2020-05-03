using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class LabourerJobDetailedReportVM
    {
        public string JobTitle { get; set; }
        public string SkillName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public DateTime Date { get; set; }
        public decimal WageAmount { get; set; }
    }
}
