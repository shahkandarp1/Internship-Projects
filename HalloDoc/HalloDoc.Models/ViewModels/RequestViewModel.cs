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
        public string Name { get; set; }

        public int count { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

        public string status_name { get; set; }
    }
}
