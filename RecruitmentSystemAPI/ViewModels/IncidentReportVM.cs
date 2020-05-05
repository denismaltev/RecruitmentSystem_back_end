using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class IncidentReportVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int JobId { get; set; }
        public string Summary { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }

        public List<IncidentReportLabourerVM> Labourers { get; set; } //id and full name

    }
}
