using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.ViewModels
{
    public class PatientRequestViewModel
    {
        public string ?Symptoms { get; set; }

        [Required(ErrorMessage = "Please enter the patient's first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the patient's last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter the patient's date of birth")]
        public DateTime DateOfBirth { get; set; }

        public string? Email { get; set; }

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

        public string ?Room { get; set; }

        [Compare("ConfirmPassword", ErrorMessage = "Password and Confirm Password should be same")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
            ErrorMessage = "Password must have at least 8 characters, including 1 uppercase letter, 1 digit, and 1 special character.")]
        public string ?Password { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
            ErrorMessage = "Confirm Password must have at least 8 characters, including 1 uppercase letter, 1 digit, and 1 special character.")]
        public string ?ConfirmPassword { get; set; }

        public IFormFile ?ImageContent { get; set; }

        //Create Request Admin Page
        public string? Admin_notes {  get; set; }

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
    }
}
