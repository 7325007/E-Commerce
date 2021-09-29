using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class ProductDetailsViewModel
    {
        [Required(ErrorMessage =" Product Name Required")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = " Price Required")]
        [Display(Name = "Price (₹)")]
        public string Price { get; set; }

        [Display(Name = "Product Image")]
        public string ProductImage { get; set; }

        public HttpPostedFileBase ImgFile { get; set; }

        public int ProductId { get; set; }

        public string Content { get; set; }
    }
}