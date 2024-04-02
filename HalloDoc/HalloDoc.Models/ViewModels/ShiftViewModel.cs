using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class ShiftViewModel
    {
        public string PhysicianName { get; set; }
        public string RegionAbbreviation { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeOnly EndTime { get; set; }
        public TimeOnly StartTime { get; set; }
        public int Status { get; set; }
        public int PhysicianId { get; set; }
        public int RegionId { get; set; }
        public int ShiftDetailId { get; set; }

    }
}
