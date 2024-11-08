using OrderProcessSystem.Models;

namespace OrderProcessSystem.Services
{
    public interface IOrderProcessorService
    {
        TransformedOrder ProcessOrder(Order order);
    }
}
