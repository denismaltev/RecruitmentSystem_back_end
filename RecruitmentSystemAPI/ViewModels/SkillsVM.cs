using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class SkillsVM
    {
        public int? Id { get; set; }

        [Required]
        [DisplayName("Name of Skill")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Amount to charge client")]
        public decimal ChargeAmount { get; set; }

        [Required]
        [DisplayName("Amount to pay labourer")]
        public decimal PayAmount { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
