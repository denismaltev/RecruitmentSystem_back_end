using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class JobSkillVM
    {
        [Required]
        [DisplayName("Skill Id")]
        public int SkillId { get; set; }

        [Required]
        [DisplayName("Name of Skill")]
        public string SkillName { get; set; }

        [Required]
        [DisplayName("Number Of Labourers Needed")]
        public int NumberOfLabourersNeeded { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
