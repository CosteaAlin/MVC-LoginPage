using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Login.Models
{
    public class User
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "This field is required.")]
        public string ConfirmPassword { get; set; }
    }
}