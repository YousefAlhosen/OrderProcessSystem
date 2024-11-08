using FluentAssertions;
using OrderProcessSystem.Models;
using OrderProcessSystem.Services;

namespace OrderProcessSystem.Tests.Services
{
    public class OrderValidationServiceTests
    {
        private readonly OrderValidationService _orderValidationService;

        public OrderValidationServiceTests()
        {
            _orderValidationService = new OrderValidationService();
        }

        [Fact]
        public void ValidateOrder_ShouldReturnValidResult_WhenOrderIsValid()
        {
            var order = new Order
            {
                OrderId = "12345",
                CustomerName = "John doe",
                CustomerEmail = "john.doe@example.com",
                ShippingAddress = "123 first st",
                OrderDate = DateTime.UtcNow.AddDays(-1),
                Items = new List<OrderItem>
                {
                    new()
                    {
                        ItemId = "item-001",
                        ProductionName = "Sample Product"
                    }
                }
            };

            var result = _orderValidationService.ValidateOrder(order);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void ValidateOrder_WithMissingFields_ShouldReturnInvalidresult()
        {
            var order = new Order
            {
                OrderId = "",
                CustomerName = "",
                CustomerEmail = "",
                ShippingAddress = "",
                OrderDate = DateTime.UtcNow.AddDays(1), // Future date to trigger validation
                Items = new List<OrderItem>()
            };

            var result = _orderValidationService.ValidateOrder(order);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("Order Id cannot be empty.");
            result.Errors.Should().Contain("Customer name cannot be empty.");
            result.Errors.Should().Contain("Customer email cannot be empty.");
            result.Errors.Should().Contain("Shipping address cannot be empty.");
            result.Errors.Should().Contain("Order must contain at least one Item.");
            result.Errors.Should().Contain("Order date cannot be in the future.");

        }
    }
}
