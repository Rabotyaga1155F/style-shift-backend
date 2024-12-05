using System.ComponentModel.DataAnnotations;

namespace StyleShiftBackend.Models;

public class DeliveryStatus
{
    [Key]
    public string StatusID { get; set; }

    [Required]
    [MaxLength(100)]
    public string StatusName { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}