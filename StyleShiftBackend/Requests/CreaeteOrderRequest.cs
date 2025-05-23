namespace StyleShiftBackend.Requests
{
    public class CreateOrderRequest
    {
        public string UserID { get; set; }
        public string ProductID { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string PickupPointID { get; set; }
        public string SellerID { get; set; }
    }

}