using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class ProviderViewModel
    {
        public AdminNavbarViewModel adminNavbarViewModel { get; set; }

        public List<Region> regions { get; set; }

        public List<PhysicianProvider> physicianproviders { get; set; }

        //pagination
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }

        //Contact Your Provider
        
        public int? ProviderId { get; set;}
        public string? message { get; set;}
        public string? communication_type { get; set;}

    }
}
