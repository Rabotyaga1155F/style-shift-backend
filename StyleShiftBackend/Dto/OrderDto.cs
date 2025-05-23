using StyleShiftBackend.Models;

namespace StyleShiftBackend.Dto;

public class OrderDto
{
    public string OrderID { get; set; }
    public string UserID { get; set; }
    public string ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string DeliveryStatusID { get; set; }
    public string SellerID { get; set; }
    public DateTime OrderDate { get; set; }
    public string ProductName { get; set; }
    public string Size { get; set; }
    public string DeliveryStatus { get; set; }
    public string PickupPointName { get; set; }
    public PickupPoint PickupPoint { get; set; } 
    public string ImageUrl { get; set; }

    public string ConfirmationCode { get; set; }
}

