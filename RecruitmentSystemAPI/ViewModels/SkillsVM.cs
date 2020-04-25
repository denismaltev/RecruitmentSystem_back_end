using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class SkillsVM : BaseSkillsVM
    {
        [Required]
        public decimal ChargeAmount { get; set; }

        [Required]
        public decimal PayAmount { get; set; }
    }
}
