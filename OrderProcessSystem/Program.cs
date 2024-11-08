using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessSystem;
using OrderProcessSystem.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpClient();

        services.AddSingleton<IOrderValidationService, OrderValidationService>();
        services.AddSingleton<IOrderProcessorService, OrderProcessorService>();

        var queueConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
        var queueName = "sbqorder";
        //RegisteredWaitHandle the actual QueueClient
        services.AddSingleton<IQueueClient>(provider =>
           new QueueClient(queueConnectionString, queueName));

        services.AddSingleton<IQueueService, QueueService>();
        services.AddLogging();
        services.AddSingleton<IJsonSerializerService, JsonSerializerService>();
        services.AddSingleton<IShipmentOrderProcessorService, ShipmentOrderProcessorService>();
    })
    .Build();

await host.RunAsync();