using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public string OrderID { get; set; }

    [Required]
    [Column("user_id")]
    public string UserID { get; set; }

    [Required]
    [Column("product_id")]
    public string ProductID { get; set; }
    
    [Required]
    [Column("size")]
    public string Size { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Required]
    [Column("pickup_point_id")]
    public string PickupPointID { get; set; }

    [ForeignKey(nameof(PickupPointID))]
    public PickupPoint PickupPoint { get; set; } = null!;

    [Column("date")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Column("total_amount")]
    public decimal TotalAmount { get; set; }

    [Column("delivery_status_id")]
    public string? DeliveryStatusID { get; set; }

    [Column("seller_id")]
    public string SellerID { get; set; }

    [Column("confirmation_code")]
    public string? ConfirmationCode { get; set; }

    [ForeignKey(nameof(UserID))]
    public CustomUser User { get; set; } = null!;

    [ForeignKey(nameof(ProductID))]
    public Product Product { get; set; } = null!;

    [ForeignKey(nameof(DeliveryStatusID))]
    public DeliveryStatus? DeliveryStatus { get; set; }

    [ForeignKey(nameof(SellerID))]
    public CustomUser Seller { get; set; } = null!;
}