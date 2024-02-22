using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class Enums
    {
        public enum Status
        {
            Unassigned = 1, Accepted = 2, MDEnRoute = 3, MDOnSite = 4, Conclude = 5, Cancelled = 6, CancelledByPatient = 7,Closed = 8,Unpaid = 9,Clear = 10
        }
    }
}
