using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StyleShiftBackend.Requests
{
    public class CreateProductRequest
    {
        [Required]
        public string SellerID { get; set; }

        [Required]
        public string CategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public IFormFile? Image { get; set; }

        [Required]
        public string SizesJson { get; set; } 
    }


    public class ProductSizeRequest
    {
        [Required]
        public string Size { get; set; }

        [Required]
        public int Stock { get; set; }
    }
}