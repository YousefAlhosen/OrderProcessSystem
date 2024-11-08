using FluentAssertions;
using OrderProcessSystem.Models;
using OrderProcessSystem.Services;


namespace OrderProcessSystem.Tests.Services
{
    public class OrderProcessorServiceTests
    {
        private readonly IOrderProcessorService _orderProcessorService;

        public OrderProcessorServiceTests()
        {
            _orderProcessorService = new OrderProcessorService();
        }

        [Fact]
        public void ProcessOrder_ShouldReturnTransformedOrder_WhenOrderIsValid()
        {
            var order = new Order
            {
                OrderId = "12345",
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                ShippingAddress = "123 First St",
                OrderDate = DateTime.Now,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ItemId = "item-001",
                        ProductionName = "Sample Product",
                        Price = 10.00m,
                        Quantity = 2
                    },
                    new OrderItem
                    {
                        ItemId = "item-002",
                        ProductionName = "Another Product",
                        Price = 20.00m,
                        Quantity = 1
                    }
                }
            };

            var result = _orderProcessorService.ProcessOrder(order);

            result.Should().NotBeNull();
            result.OrderId.Should().Be(order.OrderId);
            result.Customer.Name.Should().Be(order.CustomerName); 
            result.Customer.Email.Should().Be(order.CustomerEmail);  
            result.Items.Should().HaveCount(2);  
            result.TotalPrice.Should().Be(40.00m);  
            result.OrderStatus.Should().Be("validated");  
            result.Priority.Should().Be("normal");  
            result.ShippingInfo.Address.Should().Be(order.ShippingAddress);  
            result.ShippingInfo.ShippingMethod.Should().Be("standard");  
            result.OrderDate.Should().Be(order.OrderDate);  
        }

    }
}
