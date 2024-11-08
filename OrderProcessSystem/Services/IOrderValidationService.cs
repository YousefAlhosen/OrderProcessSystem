using OrderProcessSystem.Models;
using OrderProcessSystem.Validation;

namespace OrderProcessSystem.Services
{
    public interface IOrderValidationService
    {
        ValidationResult ValidateOrder(Order order);
    }
}
