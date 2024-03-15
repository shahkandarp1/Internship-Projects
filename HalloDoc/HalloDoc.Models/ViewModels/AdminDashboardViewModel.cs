using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class AdminDashboardViewModel
    {

        public int new_count { get; set; }
        public int pending_count { get; set; }
        public int active_count { get; set; }
        public int conclude_count { get; set; }
        public int toclose_count { get; set; }
        public int unpaid_count { get; set; }
        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }
        public List<Request> requests { get; set; } = new List<Request>();
        public List<Region> regions { get; set; } = new List<Region>();
        public string status { get; set; }

        //pagination
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }

        //Cancel Request
        public List<CaseTag> caseTags { get; set; }
        //also used in assign case 
        public int ?RequestId { get; set; }
        public string? CaseTag { get; set; }
        public string? Admin_notes { get; set; }

        //Send Link
        public string ?Mail_FirstName { get; set; }
        public string? Mail_LastName { get; set; }
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? Mail_PhoneNumber {  get; set; }
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter valid Email")]
        public string? Mail_Email { get; set; }

        //Assign case
        public int? RegionId { get; set; }
        public int? PhysicianId { get; set; } 
        public string? Description { get; set; }

        //Block Case

        public string BlockReason { get; set; }

        //export
        public string? requestor { get; set; }
        public string? search { get; set; } 
    }
}
