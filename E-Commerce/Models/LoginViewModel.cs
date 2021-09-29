using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Required User Name")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Required Password")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}