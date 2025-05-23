using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

[Table("delivery_statuses")]
public class DeliveryStatus
{
    [Key]
    [Column("id")]
    public string StatusID { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string StatusName { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}