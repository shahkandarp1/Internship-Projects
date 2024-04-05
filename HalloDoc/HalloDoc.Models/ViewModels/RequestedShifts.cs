using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class RequestedShifts
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string RegionName { get; set; }
        public int RegionId { get; set; }
        public int ShiftDetailId { get; set; }
    }
}
