using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.ViewModels
{
    public class RegisterViewModel
    {

        public int? Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm Password should be same")]
        public string ConfirmPassword { get; set; }
    }
}
