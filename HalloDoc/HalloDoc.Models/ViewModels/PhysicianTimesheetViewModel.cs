using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class PhysicianTimesheetViewModel
    {
        public AdminNavbarViewModel adminNavbarViewModel { get; set; }
        public List<TimeSheetViewModel> timesheetDetails { get; set; }
        public List<TimeSheetReimbursementViewModel> timeSheetReimbursementViewModels { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
