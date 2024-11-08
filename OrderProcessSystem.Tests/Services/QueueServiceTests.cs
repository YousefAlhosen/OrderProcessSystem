using Microsoft.Azure.ServiceBus;
using NSubstitute;
using OrderProcessSystem.Services;
using System.Text;

namespace OrderProcessSystem.Tests
{
    public class QueueServiceTests
    {
        private readonly IQueueService _queueService;
        private readonly IQueueClient _mockQueueClient;

        public QueueServiceTests()
        {
            _mockQueueClient = Substitute.For<IQueueClient>();

            _queueService = new QueueService(_mockQueueClient);

        }

        [Fact]
        public async Task SendMessageAsync_ShouldSendMessageSuccessfully()
        {
            
            var messageBody = "Test message";

            await _queueService.SendMessageAsync(messageBody);

            // Assert
            await _mockQueueClient.Received(1).SendAsync(Arg.Is<Message>(msg =>
                Encoding.UTF8.GetString(msg.Body) == messageBody));
        }

        [Fact]
        public async Task CloseAsync_ShouldCloseQueueClientSuccessfully()
        {
            await _queueService.CloseAsync();

            // Assert
            await _mockQueueClient.Received(1).CloseAsync();
        }
    }
}
