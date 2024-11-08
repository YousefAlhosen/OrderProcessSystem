namespace OrderProcessSystem.Models
{
    public class TransformedOrder
    {
        public required string OrderId { get; set; }
        public required Customer Customer { get; set; }
        public required List<OrderItem> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public required string OrderStatus { get; set; }
        public required string Priority { get; set; }
        public DateTime OrderDate { get; set; }
        public required ShippingInfo ShippingInfo { get; set; }
    }
}
