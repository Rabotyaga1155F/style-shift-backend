using System.ComponentModel.DataAnnotations;

namespace StyleShiftBackend.Models;

public class Category
{
    [Key]
    public string CategoryID { get; set; }

    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}