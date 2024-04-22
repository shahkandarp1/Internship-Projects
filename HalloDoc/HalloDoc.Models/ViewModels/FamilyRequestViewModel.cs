using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class FamilyRequestViewModel
    {
        public string? Symptoms { get; set; }

        [Required(ErrorMessage = "Please enter the patient's first name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the patient's last name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter the patient's date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the patient's phone number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter the patient's street address")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please enter the patient's city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter the patient's state")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter the patient's ZIP code")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "ZipCode must be numeric.")]
        public string ZipCode { get; set; }

        public string? Room { get; set; }

        public IFormFile? ImageContent { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string? FamilyFirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string? FamilyLastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string? FamilyEmail { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public string? FamilyPhoneNumber { get; set; }
        [Required(ErrorMessage = "Relation is required")]
        public string ?FamilyRelation { get; set; }
    }
}
