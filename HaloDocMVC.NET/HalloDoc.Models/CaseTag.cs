using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.NET;

[Table("CaseTag")]
public partial class CaseTag
{
    [Key]
    public int CaseTagId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;
}
