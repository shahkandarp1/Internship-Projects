using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class UserAccessData
    {
        public string? AccountType { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Status { get; set; }
        public int? OpenRequest { get; set; }
        public int? PhysicianId  { get; set; }
        public int? AdminId  { get; set; }
    }
}
