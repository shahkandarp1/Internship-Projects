using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class SMSLogViewModel
    {
        public string? name { get; set; }
        public int? action { get; set; }
        public string? rolename { get; set; }
        public int? roleid { get; set; }
        public string? mobilenumber { get; set; }
        public DateTime? createddate { get; set; }
        public DateTime? sentdate { get; set; }
        public string? sent { get; set; }
        public int? senttries { get; set; }
        public string? confirmationnumber { get; set; }
    }
}
