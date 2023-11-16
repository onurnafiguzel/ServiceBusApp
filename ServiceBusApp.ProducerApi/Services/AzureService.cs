using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System.Text;
using Constants = ServiceBusApp.Common.Constants;

namespace ServiceBusApp.ProducerApi.Services;

public class AzureService
{
    private readonly ManagementClient managementClient;

    public AzureService(ManagementClient managementClient)
    {
        this.managementClient = managementClient;
    }

    public async Task SendeMessageToQueue(string queueName, object messageContent, string messageType = null)
    {
        IQueueClient client = new QueueClient(Constants.ConnectionString, queueName);

        await SendMessage(client, messageContent, messageType);
    }

    public async Task CreateaQueueIfNotExist(string queueName)
    {
        if (!await managementClient.QueueExistsAsync(queueName))
            await managementClient.CreateQueueAsync(queueName);
    }

    public async Task SendMessageToTopic(string topicName, object messageContent, string messageType = null)
    {
        ITopicClient client = new TopicClient(Constants.ConnectionString, topicName);

        await SendMessage(client, messageContent, messageType);
    }

    public async Task CreateTopicIfNotExist(string topicName)
    {
        if (!await managementClient.TopicExistsAsync(topicName))
            await managementClient.CreateTopicAsync(topicName);
    }

    public async Task CreateSubscriptionIfNotExists(string topicName, string subscriptionName, string messageType = null, string ruleName = null)
    {
        if (!await managementClient.SubscriptionExistsAsync(topicName, subscriptionName))
            return;

        if (messageType is not null)
        {
            SubscriptionDescription sd = new SubscriptionDescription(topicName, subscriptionName);

            CorrelationFilter filter = new CorrelationFilter();

            filter.Properties["MessageType"] = messageType;

            RuleDescription rd = new RuleDescription(ruleName ?? messageType + "Rule", filter);

            await managementClient.CreateSubscriptionAsync(sd, rd);
        }
        else
            await managementClient.CreateSubscriptionAsync(topicName, subscriptionName);
    }

    private async Task SendMessage(ISenderClient client, object messageContent, string messageType = null)
    {
        var byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageContent));

        var message = new Message(byteArray);
        message.UserProperties["MessageType"] = messageType;

        await client.SendAsync(message);
    }
}
