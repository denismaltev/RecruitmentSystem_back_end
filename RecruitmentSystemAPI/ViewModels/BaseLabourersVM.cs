using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class BaseLabourersVM
    {
        public int? Id { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
    }
}
