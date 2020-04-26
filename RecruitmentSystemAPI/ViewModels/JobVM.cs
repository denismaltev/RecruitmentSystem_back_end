using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class JobVM
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string City { get; set; }
        public string Province { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Address { get; set; }

        public float Rating { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public bool Sunday { get; set; }
        [Required]
        public bool Monday { get; set; }
        [Required]
        public bool Tuesday { get; set; }
        [Required]
        public bool Wednesday { get; set; }
        [Required]
        public bool Thursday { get; set; }
        [Required]
        public bool Friday { get; set; }
        [Required]
        public bool Saturday { get; set; }
        public bool IsActive { get; set; }
        public List<JobSkillVM> JobSkills { get; set; }
    }
}
