using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class ViewNotesViewModel
    {
        public int RequestId { get; set; }
        public string? Admin_Note { get; set; }
        public string? Physician_Note { get; set; }
        public List<RequestStatusLog>? Transfer_Notes { get; set; }
        public string? Admin_Cancellation_Note { get; set; }
        public string? Cancellation_Note { get; set; }
    }
}
