using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class SchedulingViewModel
    {
        public AdminNavbarViewModel adminNavbarViewModel { get; set; }
        public List<Region>? regions { get; set; }
        public List<Physician>? physicians { get; set; }
        public List<ShiftViewModel>? shiftViewModels { get; set; }

        public int? RegionId { get; set; }
        public int? PhysicianId { get; set; }
        public int? ShiftDetailId { get; set; }
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today;
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }

        public List<CheckboxViewModel>? checkboxViewModels { get; set; }
        public int? Repeat { get; set; }

        public bool IsRepeat { get; set; }
    }
}
