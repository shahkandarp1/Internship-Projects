using HalloDoc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Models
{
    public class EditAccessViewModel
    {
        public AdminNavbarViewModel adminNavbarViewModel { get; set; }

        public List<CheckboxViewModel> checkboxViewModels { get; set; }

        public string? Name { get; set; }

        public int? Account_type { get; set; }
    }
}
