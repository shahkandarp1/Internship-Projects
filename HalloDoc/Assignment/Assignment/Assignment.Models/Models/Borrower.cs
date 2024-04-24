using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Models;

[Table("Borrower")]
public partial class Borrower
{
    [Key]
    public int Id { get; set; }

    public string? City { get; set; }

    [InverseProperty("Borrower")]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
