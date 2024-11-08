using OrderProcessSystem.Models;
using OrderProcessSystem.Validation;

namespace OrderProcessSystem.Services
{
    public class OrderValidationService : IOrderValidationService
    {
        public ValidationResult ValidateOrder(Order order)
        {
            var result = new ValidationResult(true);

            if (string.IsNullOrEmpty(order.OrderId))
            {
                result.IsValid = false;
                result.AddError("Order Id cannot be empty.");

            }

            if (string.IsNullOrEmpty(order.CustomerName))
            {
                result.IsValid = false;
                result.AddError("Customer name cannot be empty.");

            }

            if (string.IsNullOrEmpty(order.CustomerEmail))
            {
                result.IsValid = false;
                result.AddError("Customer email cannot be empty.");
            }

            if (string.IsNullOrEmpty(order.ShippingAddress))
            {
                result.IsValid = false;
                result.AddError("Shipping address cannot be empty.");
            }

            if (!order.Items.Any())
            {
                result.IsValid = false;
                result.AddError("Order must contain at least one Item.");
            }

            if (order.OrderDate > DateTime.UtcNow)
            {
                result.IsValid = false;
                result.AddError("Order date cannot be in the future.");
            }

            return result;
        }

    }
}
