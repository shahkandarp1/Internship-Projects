using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class ConciergeRequestViewModel
    {
        public string? Symptoms { get; set; }

        [Required(ErrorMessage = "Please enter the patient's first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the patient's last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter the patient's date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the patient's phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }

        public string? Room { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string ConciergeFirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string ConciergeLastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string ConciergeEmail { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string ConciergePhoneNumber { get; set; }
        [Required(ErrorMessage = "Property Name is required")]
        public string ConciergePropertyName { get; set; }
        [Required(ErrorMessage = "Street is required")]
        public string ConciergeStreet { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string ConciergeCity { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string ConciergeState { get; set; }
        [Required(ErrorMessage = "Zip Code is required")]
        public string ConciergeZipcode { get; set; }
    }
}
