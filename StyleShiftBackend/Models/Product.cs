using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

public class Product
{
    [Key]
    public string ProductID { get; set; }

    [Required]
    public string SellerID { get; set; }

    [Required]
    public string CategoryID { get; set; }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = null!;

    [ForeignKey(nameof(SellerID))]
    public CustomUser Seller { get; set; } = null!;

    [ForeignKey(nameof(CategoryID))]
    public Category Category { get; set; } = null!;
}