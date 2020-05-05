using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Models
{
    public class IncidentReport
    {
        public IncidentReport()
        {
            LabourerIncidentReports = new HashSet<LabourerIncidentReport>();
        }
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Summary { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool ReviewedByAdmin { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public virtual ICollection<LabourerIncidentReport> LabourerIncidentReports { get; set; }
    }
}
