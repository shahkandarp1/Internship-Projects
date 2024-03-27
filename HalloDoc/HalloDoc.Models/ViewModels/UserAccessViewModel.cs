using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class UserAccessViewModel
    {
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public List<UserAccessData>? userAccessData { get; set; }

        //pagination
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }

    }
}
