namespace OrderProcessSystem.Models
{
    public class OrderItem
    {
        public required string ItemId { get; set; }
        public required string ProductionName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
