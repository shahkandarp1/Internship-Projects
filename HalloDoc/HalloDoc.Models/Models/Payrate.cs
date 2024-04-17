using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc;

[Table("Payrate")]
public partial class Payrate
{
    public int PhysicianId { get; set; }

    [Key]
    public int PayrateId { get; set; }

    public decimal? NightShiftWeekend { get; set; }

    public decimal? Shift { get; set; }

    public decimal? HousecallNightWeekend { get; set; }

    public decimal? Phoneconsult { get; set; }

    public decimal? PhoneconsultNightWeekend { get; set; }

    public decimal? BatchTesting { get; set; }

    public decimal? Housecall { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("Payrates")]
    public virtual Physician Physician { get; set; } = null!;
}
