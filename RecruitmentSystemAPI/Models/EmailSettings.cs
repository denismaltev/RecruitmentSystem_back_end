using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentSystemAPI.Models
{
    public class EmailSettings

    {
        public string Domain { get; set; }
        public int Port { get; set; }
        public string UsernameLogin { get; set; }
        public string UsernamePassword { get; set; }
        public string FromEmail { get; set; }
        public string DisplayName { get; set; }
        public string ToEmail { get; set; }
        public string[] BccEmails { get; set; }
    }
}
