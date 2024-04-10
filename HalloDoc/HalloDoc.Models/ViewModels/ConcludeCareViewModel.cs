using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class ConcludeCareViewModel
    {
        public int? RequestId { get; set; }

        public string? patient_name { get; set; }

        public string? uploader_name { get; set; }

        public string? confirmation_number { get; set; }

        [Required]
        public string? ProviderNotes { get; set; }

        public List<RequestWiseFile>? requestWiseFiles { get; set; }

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }

    }
}
