using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class ViewCaseViewModel
    {
        public int? id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter the patient's phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the patient's email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string Email { get; set; }

        public string Region { get; set; }

        public string Address { get; set; }

        public int RequestId { get; set; }

        public int RequestClientId { get; set; }

        public int Status { get; set;}

        public int RequestTypeId { get; set; }

        public string ConfirmationNumber { get; set; }

        public string? Notes { get; set; }

        public string? Room { get; set; }

        public List<CaseTag> caseTags { get; set; }

        public string? CaseTag { get; set; }

        public string? Admin_notes { get; set; }

        public List<Region>? regions { get; set; } = new List<Region>();

        public AdminNavbarViewModel adminNavbarViewModel { get; set; }

        //Assign case
        public int? RegionId { get; set; }
        public int? PhysicianId { get; set; }
        public string? Description { get; set; }
    }
}
