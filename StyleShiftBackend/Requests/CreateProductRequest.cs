namespace StyleShiftBackend.Requests;

public class CreateProductRequest
{
    public string SellerID { get; set; }
    public string CategoryID { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = null!;
}