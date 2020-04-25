using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class JobSkillVM : BaseSkillsVM
    {
        [Required]
        public int NumberOfLabourersNeeded { get; set; }
    }
}
