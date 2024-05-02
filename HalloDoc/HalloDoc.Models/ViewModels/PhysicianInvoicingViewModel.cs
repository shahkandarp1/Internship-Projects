using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class PhysicianInvoicingViewModel
    {
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public Timesheet? timesheetDetails { get; set; }
        public Timesheet? timesheetReimbursement { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
