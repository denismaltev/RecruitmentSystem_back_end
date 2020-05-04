using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class LabourerJobReportVM
    {
        public int LabourerId { get; set; }
        public string LabourerFullName { get; set; }
        public string LabourerPhone { get; set; }
        public string LabourerEmail { get; set; }
        public decimal TotalWage { get; set; }

    }
}
