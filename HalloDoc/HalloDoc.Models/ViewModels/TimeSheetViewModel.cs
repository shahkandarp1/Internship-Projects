using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class TimeSheetViewModel
    {
        public int TimesheetDetailId { get; set; }

        public int TimesheetId { get; set; }

        public DateTime? Shiftdate { get; set; }

        public int? ShiftHours { get; set; }

        public int? Housecall { get; set; }

        public int? PhoneConsult { get; set; }

        public bool IsWeekend { get; set; }
    }
}
