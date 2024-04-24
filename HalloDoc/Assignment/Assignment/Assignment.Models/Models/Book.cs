using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Models;

[Table("Book")]
public partial class Book
{
    [Key]
    public int Id { get; set; }

    public string? BookName { get; set; }

    public string? Author { get; set; }

    public int? BorrowerId { get; set; }

    public string? BorrowerName { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? DateOfIssue { get; set; }

    public string? City { get; set; }

    public int? Genre { get; set; }

    [ForeignKey("BorrowerId")]
    [InverseProperty("Books")]
    public virtual Borrower? Borrower { get; set; }
}
