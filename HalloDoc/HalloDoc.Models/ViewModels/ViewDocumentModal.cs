using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class ViewDocumentModal
    {
        public string? name { get; set; }

        public string patient_name { get; set; }

        public string uploader_name { get; set; }

        public string confirmation_number { get; set; }

        public List<RequestWiseFile> requestWiseFiles { get; set; }

        public string? filename { get; set; }

        //View Uploads - Admin

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }

    }
}
