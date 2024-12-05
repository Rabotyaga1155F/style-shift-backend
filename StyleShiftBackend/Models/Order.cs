using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

public class Order
{
    [Key]
    public string OrderID { get; set; }

    [Required]
    public string UserID { get; set; }

    [Required]
    public string ProductID { get; set; }

    public int Quantity { get; set; }
    public string DeliveryAddress { get; set; }
    public string DeliveryCity { get; set; }
    
    public string? DeliveryComment { get; set; }
    public string DeliveryPhone { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }

    public string? DeliveryStatusID { get; set; }
    public string SellerID { get; set; } 

    [ForeignKey(nameof(UserID))]
    public CustomUser User { get; set; } = null!;

    [ForeignKey(nameof(ProductID))]
    public Product Product { get; set; } = null!;

    [ForeignKey(nameof(DeliveryStatusID))]
    public DeliveryStatus? DeliveryStatus { get; set; }

    [ForeignKey(nameof(SellerID))]
    public CustomUser Seller { get; set; } = null!;
}