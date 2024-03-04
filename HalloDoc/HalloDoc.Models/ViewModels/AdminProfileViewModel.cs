using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class AdminProfileViewModel
    {
        public string UserName { get; set; }
        public string? Password { get; set; }
        public short? status { get; set; }
        public int? role_id { get; set; }

        public List<AspNetRole> aspNetRoles { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter valid Email")]
        public string?Email { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter valid Confirm Email")]
        [Compare("Email", ErrorMessage = "Email and Confirm Email should be same")]
        public string? ConfirmEmail { get; set; }
        [Required]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? PhoneNumber { get; set; }

        public List<CheckboxViewModel>? checkboxViewModels { get; set; }

        [Required]
        public string? Address1 { get; set; }

        [Required]
        public string? Address2 { get; set; }

        [Required]
        public string? City { get; set; }
        [Required]
        public int? RegionId { get; set; }

        [Required]
        public string? ZipCode { get; set; }

        [Required]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? Alt_PhoneNumber { get; set; }

        public AdminNavbarViewModel adminNavbarViewModel { get; set; }
    }
}
