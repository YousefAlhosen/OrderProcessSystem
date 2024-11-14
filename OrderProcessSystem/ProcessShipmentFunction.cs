using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OrderProcessSystem.Models;
using OrderProcessSystem.Services;

namespace OrderProcessSystem
{
    public class ProcessShipmentFunction
    {
        private readonly ILogger<ProcessShipmentFunction> _logger;
        private readonly IJsonSerializerService _jsonSerializer;
        private readonly IShipmentOrderProcessorService _shipmentOrderProcessor;
        public ProcessShipmentFunction(ILogger<ProcessShipmentFunction> logger,
                                       IJsonSerializerService jsonSerializer,
                                       IShipmentOrderProcessorService shipmentOrderProcessor)
        {
            _logger = logger;
            _jsonSerializer = jsonSerializer;
            _shipmentOrderProcessor = shipmentOrderProcessor;
        }


        public ILogger<ProcessShipmentFunction> Logger => _logger;

        [Function("ProcessShipmentFunction")]
        public string Run(
             [ServiceBusTrigger("sbqorder", Connection = "ServiceBusConnectionString")] string queueItem)
        {
            _logger.LogInformation($"Received queue item: {queueItem}");

            var result = _shipmentOrderProcessor.ProcessOrder(queueItem);


            if (result.Status == OrderProcessingStatus.DeserializationError)
            {
                _logger.LogError("Deserialization resulted in null: queueItem is not a valid TransformedOrder.");
            }
            else if (result.Status == OrderProcessingStatus.JsonError)
            {
                _logger.LogError($"Deserialization error: {result.Message}");
            }
            else if (result.Status == OrderProcessingStatus.Processed)
            {
                _logger.LogInformation("Order is ready for shipment.");
            }

            return result.Message;
        }

    }

}
