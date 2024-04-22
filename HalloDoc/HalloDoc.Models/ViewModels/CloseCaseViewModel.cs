using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class CloseCaseViewModel
    {
        public int? RequestId { get; set; }

        public string? patient_name { get; set; }

        public string? uploader_name { get; set; }

        public string? confirmation_number { get; set; }

        public List<RequestWiseFile> requestWiseFiles { get; set; }

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public string? PhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string? Email { get; set; }
    }
}
