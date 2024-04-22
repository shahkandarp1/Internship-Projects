using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class BusinessViewModel
    {
        public int? BusinessId { get; set; }

        public List<HealthProfessionalType>? healthProfessionalTypes { get; set; }

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }

        [Required]
        public string Name { get; set; }

        public int? ProfessionId { get; set; }

        [Required]
        public string FaxNumber { get; set; }
        [Required]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter valid Email")]
        public string Email { get; set; }

        public string? BusinessContact { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }
        [RegularExpression("^[0-9]+$", ErrorMessage = "ZipCode must be numeric.")]
        public string? ZipCode { get; set; }
        public string? page { get; set; }
    }
}
