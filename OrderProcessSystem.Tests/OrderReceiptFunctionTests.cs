using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using OrderProcessSystem.Models;
using OrderProcessSystem.Services;
using OrderProcessSystem.Validation;

namespace OrderProcessSystem.Tests
{
    public class OrderReceiptFunctionTests
    {
        private readonly IOrderValidationService _validationService;
        private readonly IOrderProcessorService _processorService;
        private readonly IQueueService _queueService;
        private readonly ILogger<OrderReceiptFunction> _logger;
        private readonly OrderReceiptFunction _function;

        public OrderReceiptFunctionTests()
        {
            // Create substitutes for the dependencies using NSubstitute
            _validationService = Substitute.For<IOrderValidationService>();
            _processorService = Substitute.For<IOrderProcessorService>();
            _queueService = Substitute.For<IQueueService>();
            _logger = Substitute.For<ILogger<OrderReceiptFunction>>();

            // Create function instance with the substituted dependencies
            _function = new OrderReceiptFunction(
                _validationService,
                _processorService,
                _queueService,
                _logger
            );
        }

        [Fact]
        public async Task Run_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            HttpRequestData request = null;

            var result = await _function.Run(request);

            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("HttpRequest cannot be null.");
        }

        [Fact]
        public async Task Run_ShouldReturnBadRequest_WhenRequestBodyIsEmpty()
        {

            var request = CreateMockHttpRequest(string.Empty);


            var result = await _function.Run(request);

            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Order data cannot be null.");
        }

        [Fact]
        public async Task Run_ShouldReturnBadRequest_WhenOrderDataCannotBeDeserialized()
        {

            var invalidJson = "{ invalid order data }";
            var request = CreateMockHttpRequest(invalidJson);

            var result = await _function.Run(request);

            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Order data cannot be deserialized.");
        }

        [Fact]
        public async Task Run_ShouldReturnBadRequest_WhenOrderValidationFails()
        {

            var order = new Order
            {
                OrderId = "123",
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                Items = new List<OrderItem>
                {
                    new OrderItem { ItemId = "1", ProductionName = "Item1", Quantity = 1, Price = 100.00m }
                },
                OrderDate = DateTime.UtcNow,
                ShippingAddress = "123 Main St, City, Country"
            };

            var serializedOrder = JsonConvert.SerializeObject(order);
            var request = CreateMockHttpRequest(serializedOrder);
            _validationService.ValidateOrder(Arg.Any<Order>()).Returns(new ValidationResult(false, "Invalid order"));

            var result = await _function.Run(request);

            result.Should().BeOfType<BadRequestObjectResult>(); 

            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull(); 

            var value = badRequestResult?.Value;
            value.Should().BeAssignableTo<ValidationResult>(); 

            var validationResult = value as ValidationResult;
            validationResult?.Errors.Should().Contain("Invalid order"); 
        }

        [Fact]
        public async Task Run_ShouldProcessOrderSuccessfully_WhenValidOrderIsProvided()
        {
            var order = new Order
            {
                OrderId = "123",
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                Items = new List<OrderItem>
                {
                    new OrderItem { ItemId = "1", ProductionName = "Item1", Quantity = 1, Price = 100.00m }
                },
                OrderDate = DateTime.UtcNow,
                ShippingAddress = "123 Main St, City, Country"
            };
 
            var transformedOrder = new TransformedOrder
            {
                OrderId = order.OrderId,
                Customer = new Customer
                {
                    Name = "John Doe",
                    Email = "john.doe@example.com"
                },
                Items = order.Items,
                TotalPrice = order.Items.Sum(item => item.Price * item.Quantity),
                OrderStatus = "Processed",
                Priority = "High",
                OrderDate = order.OrderDate,
                ShippingInfo = new ShippingInfo
                {
                    Address = order.ShippingAddress,
                    ShippingMethod = "Standard"
                }
            };

            var request = CreateMockHttpRequest(JsonConvert.SerializeObject(order));

            // Setup mock behavior for validation and processing
            var validationResult = new ValidationResult(true);
            _validationService.ValidateOrder(Arg.Any<Order>()).Returns(validationResult);
            _processorService.ProcessOrder(Arg.Any<Order>()).Returns(transformedOrder);
            _queueService.SendMessageAsync(Arg.Any<string>()).Returns(Task.CompletedTask);

            var result = await _function.Run(request);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Order received and processed successfully");
            await _queueService.Received(1).SendMessageAsync(Arg.Any<string>());
        }


        private HttpRequestData CreateMockHttpRequest(string body)
        {
            var mockRequest = Substitute.For<HttpRequestData>(Substitute.For<FunctionContext>());
            var mockStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
            mockRequest.Body.Returns(mockStream);
            return mockRequest;
        }
    }
}
