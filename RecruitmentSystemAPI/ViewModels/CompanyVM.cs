using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class CompanyVM
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string City { get; set; }
        public string Province { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        public bool IsActive { get; set; }
    }
}
