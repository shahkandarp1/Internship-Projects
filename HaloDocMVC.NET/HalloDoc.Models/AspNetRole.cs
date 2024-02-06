using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.NET;

public partial class AspNetRole
{
    [Key]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [StringLength(256)]
    public string Name { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<AspNetUser> Users { get; set; } = new List<AspNetUser>();
}
