using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

public class Favorite
{
    [Key]
    public string FavoriteID { get; set; }

    [Required]
    public string UserID { get; set; }

    [Required]
    public string ProductID { get; set; }

    [ForeignKey(nameof(UserID))]
    public CustomUser User { get; set; } = null!;

    [ForeignKey(nameof(ProductID))]
    public Product Product { get; set; } = null!;
}