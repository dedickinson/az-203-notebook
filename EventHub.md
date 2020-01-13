# Event Hub

* MSFT Docs: https://docs.microsoft.com/en-us/azure/event-hubs/
* Demo: <EventHub/README.md>

Packages:

- `Microsoft.Azure.EventHubs`

Protocols:

- AMQP
- Kafka

## Connection String

    Endpoint=sb://az203-dd-eh.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<KEY>

## CLI

    az eventhubs

    az eventhubs namespace list

    az eventhubs namespace create --name MyNamespace --resource-group az-203 --sku Basic

## Sample Code

Note that an EventHub Namespace contains Event Hubs (also called entity paths).

Connect:

```C#
private static EventHubClient eventHubClient;

var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
{
    EntityPath = EventHubName
};

eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
```

Send a message:

```C#
await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
```

Disconnect:

```C#
await eventHubClient.CloseAsync();
```

### Subscriber

Setup the subscriber - note that a storage account is used for check marking:

```C#
private static async Task MainAsync(string[] args)
{
    Console.WriteLine("Registering EventProcessor...");

    var eventProcessorHost = new EventProcessorHost(
        EventHubName,
        PartitionReceiver.DefaultConsumerGroupName,
        EventHubConnectionString,
        StorageConnectionString,
        StorageContainerName);

    // Registers the Event Processor Host and starts receiving messages
    await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

    Console.WriteLine("Receiving. Press ENTER to stop worker.");
    Console.ReadLine();

    // Disposes of the Event Processor Host
    await eventProcessorHost.UnregisterEventProcessorAsync();
}
```

Basic event processor class:

```C#
public class SimpleEventProcessor : IEventProcessor
{
    public Task CloseAsync(PartitionContext context, CloseReason reason)
    {
        Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
        return Task.CompletedTask;
    }

    public Task OpenAsync(PartitionContext context)
    {
        Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
        return Task.CompletedTask;
    }

    public Task ProcessErrorAsync(PartitionContext context, Exception error)
    {
        Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
        return Task.CompletedTask;
    }

    public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
    {
        foreach (var eventData in messages)
        {
            var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
            Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");
        }

        return context.CheckpointAsync();
    }
}
```
