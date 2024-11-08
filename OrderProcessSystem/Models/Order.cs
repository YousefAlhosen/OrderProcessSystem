namespace OrderProcessSystem.Models
{
    public class Order
    {
        public required string OrderId { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerEmail { get; set; }
        public required List<OrderItem> Items { get; set; }
        public DateTime OrderDate { get; set; }
        public required string ShippingAddress { get; set; }
    }
}
