using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc;

[Table("PasswordReset")]
public partial class PasswordReset
{
    [Key]
    public string Token { get; set; } = null!;

    public string Email { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("isUpdated")]
    public bool? IsUpdated { get; set; }
}
