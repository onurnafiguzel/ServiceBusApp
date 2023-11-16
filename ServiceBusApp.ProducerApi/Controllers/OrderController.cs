using Microsoft.AspNetCore.Mvc;
using ServiceBusApp.Common;
using ServiceBusApp.Common.Dto;
using ServiceBusApp.Common.Events;
using ServiceBusApp.ProducerApi.Services;

namespace ServiceBusApp.ProducerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly AzureService azureService;

    public OrderController(AzureService service)
    {
        azureService = service;
    }

    [HttpPost]
    public async Task CreateOrder(OrderDto order)
    {
        var orderCreatedEvent = new OrderCreatedEvent()
        {
            Id = order.Id,
            CreatedOn = DateTime.Now,
            ProductName = order.ProductName
        };

        #region WithQueue

        await azureService.CreateaQueueIfNotExist(Constants.OrderCreatedQueue);
        await azureService.SendeMessageToQueue(Constants.OrderCreatedQueue, orderCreatedEvent);
        #endregion

        #region WithTopic

        await azureService.CreateTopicIfNotExist(Constants.OrderTopic);
        await azureService.CreateSubscriptionIfNotExists(Constants.OrderTopic, Constants.OrderCreatedSubName, "OrderCreated", "OrderCreatedOnly");
        await azureService.SendMessageToTopic(Constants.OrderTopic, orderCreatedEvent, "OrderCreated");

        #endregion

    }

    [HttpDelete]
    public async Task DeleteOrder(int id)
    {
        var orderDeletedEvent = new OrderDeletedEvent()
        {
            Id = id,
            CreatedOn = DateTime.Now
        };

        #region WithQueue       

        await azureService.CreateaQueueIfNotExist(Constants.OrderDeletedQueue);
        await azureService.SendeMessageToQueue(Constants.OrderDeletedQueue, orderDeletedEvent);
        #endregion

        #region WithTopic

        await azureService.CreateTopicIfNotExist(Constants.OrderTopic);
        await azureService.CreateSubscriptionIfNotExists(Constants.OrderTopic, Constants.OrderDeletedSubName, "OrderDeleted", "OrderDeletedOnly");
        await azureService.SendMessageToTopic(Constants.OrderTopic, orderDeletedEvent, "OrderDeleted");
        #endregion
    }
}
