using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class PayRateViewModel
    {
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public decimal? NightShiftWeekend { get; set; }
        public decimal? Shift { get; set; }
        public decimal? HouseCalls_Night_Weekend { get; set; }
        public decimal? PhoneConsult_Night_Weekend { get; set; }
        public decimal? BatchTesting { get; set; }
        public decimal? PhoneConsult { get; set; }
        public decimal? HouseCall { get; set; }
    }
}
