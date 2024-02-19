using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc;

[Table("Family")]
public partial class Family
{
    [Column("firstname")]
    [StringLength(100)]
    public string? Firstname { get; set; }

    [Column("lastname")]
    [StringLength(100)]
    public string? Lastname { get; set; }

    [Column("mobile")]
    [StringLength(20)]
    public string? Mobile { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("relation")]
    [StringLength(100)]
    public string? Relation { get; set; }

    [Key]
    [Column("familyid")]
    public int Familyid { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [InverseProperty("Family")]
    public virtual ICollection<RequestFamily> RequestFamilies { get; set; } = new List<RequestFamily>();
}
