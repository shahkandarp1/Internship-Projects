using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class PhysicianProvider
    {
        public bool isStopNotification { get; set; }
        
        public string? name { get; set; }

        public string? role { get; set; }

        public string? oncallstatus { get; set; }

        public short? status { get; set; }

        public int? physicianId { get; set; }

    }
}
