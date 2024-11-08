using Microsoft.Extensions.Logging;
using OrderProcessSystem.Models;

namespace OrderProcessSystem.Services
{
    public class ShipmentOrderProcessorService : IShipmentOrderProcessorService
    {
        private readonly IJsonSerializerService _jsonSerializer;
        private readonly ILogger<ShipmentOrderProcessorService> _logger;

        public ShipmentOrderProcessorService(IJsonSerializerService jsonSerializer, ILogger<ShipmentOrderProcessorService> logger)
        {
            _jsonSerializer = jsonSerializer;
            _logger = logger;
        }

        public OrderProcessingResult ProcessOrder(string queueItem)
        {
            try
            {
                var transformedOrder = _jsonSerializer.Deserialize<TransformedOrder>(queueItem);

                if (transformedOrder != null)
                {
                    transformedOrder.OrderStatus = "ready for shipment";
                    return new OrderProcessingResult
                    {
                        Status = OrderProcessingStatus.Processed,
                        Message = "Processed"
                    };
                }
                else
                {
                    return new OrderProcessingResult
                    {
                        Status = OrderProcessingStatus.DeserializationError,
                        Message = "DeserializationError"
                    };
                }
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                _logger.LogError($"Deserialization error: {ex.Message}");
                return new OrderProcessingResult
                {
                    Status = OrderProcessingStatus.JsonError,
                    Message = "JsonError"
                };
            }
        }
    }

    // Enum for order processing status
    public enum OrderProcessingStatus
    {
        Processed,
        DeserializationError,
        JsonError
    }

    // Result object to hold status and message
    public class OrderProcessingResult
    {
        public OrderProcessingStatus Status { get; set; }
        public string Message { get; set; }
    }
    
    
}
