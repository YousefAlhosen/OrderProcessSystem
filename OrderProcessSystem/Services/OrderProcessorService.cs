using OrderProcessSystem.Models;

namespace OrderProcessSystem.Services
{
    public class OrderProcessorService : IOrderProcessorService
    {
        public TransformedOrder ProcessOrder(Order order)
        {
            var transformedOrder = new TransformedOrder
            {
                OrderId = order.OrderId,
                Customer = new Customer { Name = order.CustomerName, Email = order.CustomerEmail },
                Items = order.Items,
                TotalPrice = order.Items.Sum(item => item.Price * item.Quantity),
                OrderStatus = "validated",
                Priority = "normal",
                OrderDate = order.OrderDate,
                ShippingInfo = new ShippingInfo { Address = order.ShippingAddress, ShippingMethod = "standard" }
            };

            return transformedOrder;
        }
    }
}
