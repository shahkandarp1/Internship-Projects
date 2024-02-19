using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class BusinessRequestViewModel
    {
        public string? Symptoms { get; set; }

        [Required(ErrorMessage = "Please enter the patient's first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the patient's last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter the patient's date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the patient's phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter the patient's street address")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please enter the patient's city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter the patient's state")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter the patient's ZIP code")]
        public string ZipCode { get; set; }

        public string? Room { get; set; }

        [Required(ErrorMessage = "Your First Name is required")]
        public string BusinessFirstName { get; set; }
        [Required(ErrorMessage = "Your Last Name is required")]
        public string BusinessLastName { get; set; }
        [Required(ErrorMessage = "Your Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string BusinessEmail { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string BusinessPhoneNumber { get; set; }
        [Required(ErrorMessage = "Property Name is required")]
        public string BusinessPropertyName { get; set; }

        public string BusinessCaseNumber { get; set; }
    }
}
