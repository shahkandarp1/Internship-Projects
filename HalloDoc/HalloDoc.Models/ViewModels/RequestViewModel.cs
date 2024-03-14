using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc;

namespace HalloDoc.ViewModels
{
    public class RequestViewModel
    {
        public string PtName { get; set; }
        public int RequestId { get; set; }
        public string ConfirmationNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public string Name { get; set; }
        public DateTime? ConcludedDate { get; set; }
        public long? Count { get; set; }
    }
}
