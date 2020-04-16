using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class AdminSkillsVM
    {
        [Required]
        [DisplayName("Name of Skill")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Amount to charge client")]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal ChargeAmount { get; set; }

        [Required]
        [DisplayName("Amount to pay labourer")]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal PayAmount { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
