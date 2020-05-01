using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class LabourerJobReportVM
    {
        public int Id { get; set; }
        public int? JobId { get; set; }
        public string JobTitle { get; set; }
        public string LabourerFullName { get; set; }
        public DateTime Date { get; set; }
        public decimal WageAmount { get; set; }
        public decimal ChargeAmount { get; set; }
    }
}
