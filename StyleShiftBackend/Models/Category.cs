using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

[Table("categories")]
public class Category
{
    [Key]
    [Column("id")]
    public string CategoryID { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string CategoryName { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}