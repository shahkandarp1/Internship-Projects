using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Models.ViewModels
{
    public class AddBookViewModel
    {
        [Required]
        public string? BookName { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? BorrowerName { get; set; }
        [Required]
        public DateTime? DateOfIssue { get; set; }
        [Required]
        public int? Genre { get; set; }
        [Required]
        public string? City { get; set; }
        public int? BookId { get; set; }
    }
}
