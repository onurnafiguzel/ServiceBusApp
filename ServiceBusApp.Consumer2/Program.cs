using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using ServiceBusApp.Common;
using ServiceBusApp.Common.Events;
using System.Text;

ConsumeQueue<OrderCreatedEvent>(Constants.OrderCreatedQueue, i =>
{
    Console.WriteLine($"OrderCreatedEvent ReceivedMessage with id: {i.Id}, Name: {i.ProductName}");
}).Wait();

ConsumeQueue<OrderDeletedEvent>(Constants.OrderDeletedQueue, i =>
{
    Console.WriteLine($"OrderDeletedEvent ReceivedMessage with id: {i.Id}");
}).Wait();

Console.ReadLine();

static async Task ConsumeQueue<T>(string queueName, Action<T> receivedAction)
{
    IQueueClient client = new QueueClient(Constants.ConnectionString, queueName);

    client.RegisterMessageHandler(async (message, ct) =>
    {
        var model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));

        receivedAction(model);

        await Task.CompletedTask;
    },
    new MessageHandlerOptions(i => Task.CompletedTask));

    Console.WriteLine($"{typeof(T).Name} is listening...");
}

