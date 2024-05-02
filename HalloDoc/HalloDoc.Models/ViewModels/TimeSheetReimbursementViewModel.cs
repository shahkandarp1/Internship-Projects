using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class TimeSheetReimbursementViewModel
    {
        public int TimesheetReimbursementId { get; set; }

        public int TimesheetId { get; set; }

        public string Item { get; set; } = null!;

        public int Amount { get; set; }

        public string? Bill { get; set; }

        public IFormFile? File { get; set; }

        public DateTime? Date { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
