namespace OrderProcessSystem.Services
{
    public interface IShipmentOrderProcessorService
    {
        OrderProcessingResult ProcessOrder(string queueItem);
    }
}
