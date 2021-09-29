using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name ")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name ")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email Address ")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(10, MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "Mobile Number ")]
        [DataType(DataType.PhoneNumber)]
        public string MobileNo { get; set; }

        [Display(Name = "Admin ")]
        public bool IsAdmin { get; set; }

        public int UserId { get; set; }
    }
}