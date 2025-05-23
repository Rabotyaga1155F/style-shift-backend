using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

[Table("products")]
public class Product
{
    [Key]
    [Column("id")]
    public string ProductID { get; set; }

    [Required]
    [Column("seller_id")]
    public string SellerID { get; set; }

    [Required]
    [Column("category_id")]
    public string CategoryID { get; set; }

    [Column("title")]
    public string Title { get; set; } = null!;
    
    [Column("description")]
    public string? Description { get; set; }

    [Column("price")]
    public decimal Price { get; set; }
    
    [Column("views")]
    public int Views { get; set; }

    [Column("image_url")]
    public string ImageUrl { get; set; } = null!;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [ForeignKey(nameof(SellerID))]
    public CustomUser Seller { get; set; } = null!;

    [ForeignKey(nameof(CategoryID))]
    public Category Category { get; set; } = null!;
    
    public ICollection<ProductSize> Sizes { get; set; } = new List<ProductSize>();
}
