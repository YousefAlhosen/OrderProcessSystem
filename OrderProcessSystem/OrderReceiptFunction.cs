using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderProcessSystem.Models;
using OrderProcessSystem.Services;
using OrderProcessSystem.Validation;

namespace OrderProcessSystem
{
    public class OrderReceiptFunction
    {
        private readonly  IOrderValidationService _validationService;
        private readonly  IOrderProcessorService _processorService;
        private readonly  IQueueService _queueService;
        private readonly  ILogger<OrderReceiptFunction> _logger;

        public OrderReceiptFunction(IOrderValidationService validationService, IOrderProcessorService processorService, IQueueService queueService, ILogger<OrderReceiptFunction> logger)
        {
            this._validationService = validationService ;
            this._processorService = processorService ;
            this._queueService = queueService ;
            this._logger = logger ;
        }


        [Function("OrderReceiptFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "order")] HttpRequestData req)
        {
            if (req == null)
            {
                return await HandleInvalidRequest("HttpRequest cannot be null.");
            }


            string requestBody = await ReadRequestBody(req);

            if (string.IsNullOrEmpty(requestBody))
            {
                return await HandleInvalidRequest("Order data cannot be null.");
            }


            var order = DeserializeOrder(requestBody);

            if (order == null)
            {
                return await HandleInvalidRequest("Order data cannot be deserialized.");
            }


            var validationResult = _validationService.ValidateOrder(order);

            if (!validationResult.IsValid)
            {
                return await HandleValidationFailure(validationResult.Errors);
            }

            var transformedOrder = _processorService.ProcessOrder(order);
            await _queueService.SendMessageAsync(JsonConvert.SerializeObject(transformedOrder));

            _logger.LogInformation($"Order {transformedOrder.OrderId} has been sent to the Service Bus queue.");

            return new OkObjectResult("Order received and processed successfully");
        }

        private async Task<IActionResult> HandleInvalidRequest(string errorMessage)
        {
            _logger.LogWarning(errorMessage);
            return await Task.FromResult(new BadRequestObjectResult(errorMessage));
        }

        private async Task<IActionResult> HandleValidationFailure(IEnumerable<string> errors)
        {
            var validationResult = new ValidationResult(false, errors.ToArray());
            _logger.LogWarning("Order validation failed: {Errors}", string.Join(", ", errors));
            return await Task.FromResult(new BadRequestObjectResult(validationResult));
        }

        private async Task<string> ReadRequestBody(HttpRequestData req)
        {
            using var reader = new StreamReader(req.Body);
            return await reader.ReadToEndAsync();
        }

        private Order DeserializeOrder(string requestBody)
        {
            try
            {
                return JsonConvert.DeserializeObject<Order>(requestBody);
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError($"Error deserializing order: {ex.Message}");
                return null;  // Return null if deserialization fails
            }
        }
    }
}
