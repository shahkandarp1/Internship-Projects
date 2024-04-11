using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class EmailLogViewModel
    {
        public List<AspNetRole> roles {  get; set; }

        public AdminNavbarViewModel adminNavbarViewModel { get; set; }

        public List<ELogViewModel> ?logViewModels { get; set; }
        public List<SMSLogViewModel> ?smsLogViewModels { get; set; }

        //pagination
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }
    }
}
