namespace OrderProcessSystem.Services
{
    public interface IQueueService
    {
        Task SendMessageAsync(string messageBody);
        Task CloseAsync();
    }
}
