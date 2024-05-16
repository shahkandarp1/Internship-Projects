using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc;

namespace HalloDoc.ViewModels
{
    public class DashboardViewModel
    {
        public string name { get; set; }
        public int? aspid { get; set; }

        public List<RequestViewModel> requests { get; set; }

        //pagination
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }
       
    }

}
