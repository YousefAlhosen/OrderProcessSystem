using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderProcessSystem.Services;

namespace OrderProcessSystem.Tests
{
    public class ProcessShipmentFunctionTests
    {
        private readonly ILogger<ProcessShipmentFunction> _mockLogger;
        private readonly IJsonSerializerService _mockJsonSerializer;
        private readonly IShipmentOrderProcessorService _mockOrderProcessor;
        private readonly ProcessShipmentFunction _processShipmentFunction;

        public ProcessShipmentFunctionTests()
        {
            _mockLogger = Substitute.For<ILogger<ProcessShipmentFunction>>();
            _mockJsonSerializer = Substitute.For<IJsonSerializerService>();
            _mockOrderProcessor = Substitute.For<IShipmentOrderProcessorService>();

            // Initialize the function with substituted dependencies
            _processShipmentFunction = new ProcessShipmentFunction(
                _mockLogger,
                _mockJsonSerializer,
                _mockOrderProcessor
            );
        }


        [Fact]
        public void Run_DeserializationError_ReturnsDeserializationErrorMessage()
        {
            var queueItem = "invalid json"; 
            var orderProcessingResult = new OrderProcessingResult
            {
                Status = OrderProcessingStatus.DeserializationError,
                Message = "DeserializationError"
            };

            _mockOrderProcessor.ProcessOrder(Arg.Any<string>()).Returns(orderProcessingResult);

            var result = _processShipmentFunction.Run(queueItem);

            result.Should().Be("DeserializationError");
            _mockLogger.Received().LogError("Deserialization resulted in null: queueItem is not a valid TransformedOrder.");
        }

        [Fact]
        public void Run_JsonError_ReturnsJsonErrorMessage()
        {
            var queueItem = "{\"OrderId\": 1, \"CustomerId\": 123}"; 
            var orderProcessingResult = new OrderProcessingResult
            {
                Status = OrderProcessingStatus.JsonError,
                Message = "JsonError"
            };

            _mockOrderProcessor.ProcessOrder(Arg.Any<string>()).Returns(orderProcessingResult);

            var result = _processShipmentFunction.Run(queueItem);

            result.Should().Be("JsonError");
            _mockLogger.Received().LogError("Deserialization error: JsonError");
        }

        [Fact]
        public void Run_ProcessedOrder_ReturnsProcessedMessage()
        {
            var queueItem = "{\"OrderId\": 1, \"CustomerId\": 123}";
            var orderProcessingResult = new OrderProcessingResult
            {
                Status = OrderProcessingStatus.Processed,
                Message = "Processed"
            };

            _mockOrderProcessor.ProcessOrder(Arg.Any<string>()).Returns(orderProcessingResult);

            var result = _processShipmentFunction.Run(queueItem);

            result.Should().Be("Processed");
            _mockLogger.Received().LogInformation("Order is ready for shipment.");
        }
    }
}
