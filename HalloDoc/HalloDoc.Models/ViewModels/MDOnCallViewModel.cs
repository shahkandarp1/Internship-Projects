using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class MDOnCallViewModel
    {
        public List<MDOnCallPhysicians> activePhysicians { get; set; }
        public List<Region> regions { get; set; }
        public List<MDOnCallPhysicians> notActivePhysicians { get; set; }
        public AdminNavbarViewModel adminNavbarViewModel { get; set; }
    }
}
