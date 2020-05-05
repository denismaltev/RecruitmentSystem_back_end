using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Models
{
    public class LabourerIncidentReport
    {
        public int Id { get; set; }
        public int IncidentReportId { get; set; }
        public IncidentReport IncidentReport { get; set; }
        public int LabourerId { get; set; }
        public Labourer Labourer { get; set; }
    }
}
