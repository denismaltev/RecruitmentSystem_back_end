using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class BaseJobsVM
    {
        public int? JobId { get; set; }
        public string JobTitle { get; set; }
        public decimal WageAmount { get; set; }
    }
}
