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
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string? Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
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
        public string? Zipcode { get; set; }
        
        [Required]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? AltPhone { get; set; }
        
        [Required]
        public string? BusinessName { get; set; }
        
        [Required]
        public string? BusinessWebsite { get; set; }

        [Required]
        public IFormFile? Photo { get; set; }

        [Required]
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
        public string? Password { get; set; }

        public string? UserName { get; set; }

        public int? Status { get; set; }

        public int? role_id { get; set; }

        public List<Role>? roles { get; set; } 

        public int? PhysicianId { get; set; }

        public int? AspId { get; set; }

        public string? signature_name { get; set; }
    }
}
