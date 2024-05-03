using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class InvoiceTotalViewModal
    {
        public decimal? TotalHours { get; set; }
        public decimal? TotalWeekend { get; set; }
        public decimal? TotalHousecall { get; set; }
        public decimal? TotalPhoneconsult { get; set; }
    }
}
