using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc;

public partial class AspNetUser
{
    [Key]
    public int Id { get; set; }

    [StringLength(256)]
    public string UserName { get; set; } = null!;

    public string? PasswordHash { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Admin> AdminAspNetUsers { get; set; } = new List<Admin>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Admin> AdminCreatedByNavigations { get; set; } = new List<Admin>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Admin> AdminModifiedByNavigations { get; set; } = new List<Admin>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Business> BusinessCreatedByNavigations { get; set; } = new List<Business>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Business> BusinessModifiedByNavigations { get; set; } = new List<Business>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Physician> PhysicianAspNetUsers { get; set; } = new List<Physician>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Physician> PhysicianCreatedByNavigations { get; set; } = new List<Physician>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Physician> PhysicianModifiedByNavigations { get; set; } = new List<Physician>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<RequestNote> RequestNoteCreatedByNavigations { get; set; } = new List<RequestNote>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<RequestNote> RequestNoteModifiedByNavigations { get; set; } = new List<RequestNote>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<User> UserAspNetUsers { get; set; } = new List<User>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<User> UserCreatedByNavigations { get; set; } = new List<User>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<User> UserModifiedByNavigations { get; set; } = new List<User>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
