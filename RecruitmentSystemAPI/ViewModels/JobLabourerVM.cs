using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.ViewModels
{
    public class JobLabourerVM
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string SkillName { get; set; }
    }
}
