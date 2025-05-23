using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

[Table("product_sizes")]
public class ProductSize
{
    [Key]
    [Column("id")]
    public string ProductSizeID { get; set; }= Guid.NewGuid().ToString();

    [Required]
    [Column("product_id")]
    public string ProductID { get; set; }

    [Required]
    [Column("size")]
    public string Size { get; set; } = null!;

    [Column("stock")]
    public int Stock { get; set; }

    [ForeignKey(nameof(ProductID))]
    public Product Product { get; set; } = null!;
}
