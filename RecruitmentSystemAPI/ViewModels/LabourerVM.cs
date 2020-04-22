using RecruitmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class LabourerVM
    {
         public int? Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PersonalId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        public bool IsActive { get; set; }

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
        public List<SkillsVM> Skills { get; set; }
        public float SafetyRating { get; set; }
        public float QualityRating { get; set; }
    }
}
