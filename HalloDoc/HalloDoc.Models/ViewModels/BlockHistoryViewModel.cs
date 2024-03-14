using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class BlockHistoryViewModel
    {
        public List<BlockRequest>? blockRequests { get; set; }

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }

        //pagination
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }
    }
}
