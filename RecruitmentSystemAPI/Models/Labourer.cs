using System;
using System.Collections.Generic;

namespace RecruitmentSystemAPI.Models
{
    public partial class Labourer
    {
        public Labourer()
        {
            LabourerSkills = new HashSet<LabourerSkill>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public Weekdays Availability { get; set; }

        public float SafetyRating { get; set; }
        public float QualityRating { get; set; }

        public virtual SystemUser User { get; set; }
        public virtual ICollection<LabourerSkill> LabourerSkills { get; set; }
        public virtual ICollection<LabourerJob> LabourerJobs { get; set; }
    }
}
