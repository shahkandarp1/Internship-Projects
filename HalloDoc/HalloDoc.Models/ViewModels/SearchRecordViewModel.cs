using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class SearchRecordViewModel
    {

        public List<Request> requests { get; set; }

        public List<Request> alldata { get; set; }

        public List<RequestType> requestTypes { get; set; }

        public AdminNavbarViewModel adminNavbarViewModel { get; set; }

        //pagination
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }

        //Excel
        public short? status { get; set; }
        public string? name { get; set; }
        public int? requesttypeid { get; set; }
        public DateTime? fromdos { get; set; }
        public DateTime? todos { get; set; }
        public string? providername { get; set; }
        public string? email { get; set; }
        public string? phonenumber { get; set; }


    }
}
