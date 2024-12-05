namespace StyleShiftBackend.Requests;

public class CreaeteOrderRequest
{
    public string UserID { get; set; } = string.Empty;
    public string ProductID { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string DeliveryAddress { get; set; }
    public string DeliveryCity { get; set; }
    
    public string? DeliveryComment { get; set; }
    public string DeliveryPhone { get; set; }
    
    public string SellerID { get; set; } 
}