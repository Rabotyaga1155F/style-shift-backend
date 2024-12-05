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
    public string DeliveryStatus { get; set; }
    public string DeliveryAddress { get; set; }
    public string DeliveryCity { get; set; }
    
    public string? DeliveryComment { get; set; }
    public string DeliveryPhone { get; set; }
    public string ImageUrl { get; set; }
}
