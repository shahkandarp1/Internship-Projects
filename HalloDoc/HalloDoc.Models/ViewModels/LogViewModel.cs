using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class LogViewModel
    {
        public string? Name { get; set; }
        public string? Action { get; set; }
        public string? RoleName { get; set; }
        public string? EmailId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SentDate { get; set; }
        public string? Sent { get; set; }
        public int? SentTries { get; set; }
        public string? ConfirmationNumber { get; set; }
    }
}
