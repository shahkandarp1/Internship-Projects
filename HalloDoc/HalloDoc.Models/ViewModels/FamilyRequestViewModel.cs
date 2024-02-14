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

        [Required(ErrorMessage = "Please enter the patient's street address")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please enter the patient's city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter the patient's state")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter the patient's ZIP code")]
        public string ZipCode { get; set; }

        public string? Room { get; set; }

        public IFormFile? ImageContent { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string? FamilyFirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string? FamilyLastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? FamilyEmail { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [Phone(ErrorMessage = "Please enter phone number")]
        public string? FamilyPhoneNumber { get; set; }
        [Required(ErrorMessage = "Relation is required")]
        public string ?FamilyRelation { get; set; }
    }
}
