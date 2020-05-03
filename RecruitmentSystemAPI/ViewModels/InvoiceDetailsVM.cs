using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class InvoiceDetailsVM
    {
        public string JobTitle { get; set; }
        public string SkillName { get; set; }
        public string LabourerName { get; set; }
        public string LabourerPhone { get; set; }
        public string LabourerEmail { get; set; }
        public DateTime Date { get; set; }
        public decimal ChargeAmount { get; set; }
    }
}
