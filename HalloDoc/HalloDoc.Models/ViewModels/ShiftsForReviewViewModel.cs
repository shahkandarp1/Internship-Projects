using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class ShiftsForReviewViewModel
    {
        public AdminNavbarViewModel adminNavbarViewModel { get; set; }
        public List<RequestedShifts> requestedShifts { get; set; }
        public List<Region> regions { get; set; }
        public string? ShiftDetailIds { get; set; }

        //pagination
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }

    }
}
