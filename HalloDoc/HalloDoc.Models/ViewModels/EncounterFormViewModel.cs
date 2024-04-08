using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class EncounterFormViewModel
    {

        public int? RequestId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Location { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public DateTime? Date { get; set; } = DateTime.Now;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public string? Illness_history { get; set; }

        public string? Medical_history { get; set; }

        public string? Medications { get; set; }

        public string? Allergies { get; set; }

        public decimal? Temp { get; set; }

        public decimal? Hr { get; set; }

        public decimal? Rr { get; set; }

        public int? BpS { get; set; }

        public int? BpD { get; set; }

        public decimal? O2 { get; set; }

        public string? Pain { get; set; }

        public string? Heent { get; set; }

        public string? Cv { get; set; }

        public string? Chest { get; set; }

        public string? Abd { get; set; }

        public string? Extr { get; set; }

        public string? Skin { get; set; }

        public string? Neuro { get; set; }

        public string? Other { get; set; }

        public string? Diagnosis { get; set; }

        public string? TreatmentPlan { get; set; }

        public string? MedicationDispensed { get; set; }

        public string? Procedures { get; set; }

        public string? FollowUp { get; set; }
        public bool? isFinalized { get; set; } = false;

        public AdminNavbarViewModel? adminNavbarViewModel { get; set; }

    }
}
