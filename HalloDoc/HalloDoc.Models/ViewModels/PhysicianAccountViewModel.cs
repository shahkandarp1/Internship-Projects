using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class PhysicianAccountViewModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string FirstName { get; set; }
        
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string LastName { get; set; }
        
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public string? PhoneNumber { get; set; }

        [Required]
        public string? MedicalLicense { get; set; }

        [Required]
        public string? NPI_Number { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string? Sync_Email { get; set; }

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
        [RegularExpression("^[0-9]+$", ErrorMessage = "ZipCode must be numeric.")]
        public string? Zipcode { get; set; }
        
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public string? AltPhone { get; set; }
        
        [Required]
        public string? BusinessName { get; set; }
        
        [Required]
        [RegularExpression(@"^(https?://)?(www\.)?[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}(/\S*)?$", ErrorMessage = "Invalid website URL.")]

        public string? BusinessWebsite { get; set; }

        public IFormFile? Photo { get; set; }

        public IFormFile? Signature { get; set; }

        public string? Admin_Notes { get; set; }

        //License Document
        public bool IsLicenseDoc { get; set; } = false;
        public IFormFile? LicenseDoc { get; set; }

        //Independent Contractor agreement
        public bool IsAgreementDoc { get; set; } = false;
        public IFormFile? AgreementDoc { get; set; }

        //Background check
        public bool IsBackgroundDoc { get; set; } = false;
        public IFormFile? BackgroundDoc { get; set; }

        //Non disclosure agreement
        public bool IsNonDisclosureDoc { get; set; } = false;
        public IFormFile? NonDisclosureDoc { get; set; }

        //HIPAA Compliance
        public bool IsCredentialDoc { get; set; } = false;
        public IFormFile? CredentialDoc { get; set; }

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }

        public decimal? lat { get; set; }

        public decimal? lng { get; set; }

        //Edit Physician
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
            ErrorMessage = "Password must have at least 8 characters, including 1 uppercase letter, 1 digit, and 1 special character.")]
        public string? Password { get; set; }

        public string? UserName { get; set; }

        public int? Status { get; set; }

        public int? role_id { get; set; }

        public List<Role>? roles { get; set; } 

        public int? PhysicianId { get; set; }

        public int? AspId { get; set; }

        public string? signature_name { get; set; }

        //Doctor Profile
        public string? editReason { get; set; }

    }
}
