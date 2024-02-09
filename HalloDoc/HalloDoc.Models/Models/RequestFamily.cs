using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc;

[Table("RequestFamily")]
public partial class RequestFamily
{
    [Key]
    public int Id { get; set; }

    [Column("requestid")]
    public int? Requestid { get; set; }

    [Column("familyid")]
    public int? Familyid { get; set; }

    [ForeignKey("Familyid")]
    [InverseProperty("RequestFamilies")]
    public virtual Family? Family { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("RequestFamilies")]
    public virtual Request? Request { get; set; }
}
