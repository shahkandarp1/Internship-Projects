using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class OrdersViewModel
    {

        public List<HealthProfessionalType>? healthProfessionalTypes { get; set; }

        public int? RequestId { get; set; }

        [Required]
        [Phone(ErrorMessage = "Please enter a valid contact number")]
        public string Business_contact { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string Business_email { get; set; }
        [Required]
        public string Business_fax { get; set; }
        [Required]
        public string prescription { get; set; }
        [Required]
        public int numberOfRefills { get; set; }
        [Required]
        public int profession_id { get; set; }
        [Required]
        public int business_id { get; set; }

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    }
}
