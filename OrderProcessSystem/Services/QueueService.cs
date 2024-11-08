using Microsoft.Azure.ServiceBus;
using System.Text;

namespace OrderProcessSystem.Services
{
    public class QueueService : IQueueService
    {
        private readonly IQueueClient _queueClient;

        public QueueService(IQueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public async Task SendMessageAsync(string messageBody)
        {
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await _queueClient.SendAsync(message);
        }

        public async Task CloseAsync()
        {
            await _queueClient.CloseAsync();
        }
    }
}
