# Service Bus

* MSFT Docs: https://docs.microsoft.com/en-us/azure/service-bus/
* Demo: [ServiceBus](ServiceBus)

A Service Bus _namespace_ contains:

* Queues (all plans)
* Topics (Standard and higher)

Required packages:

- `Microsoft.Azure.ServiceBus`

Protocols:

- AMQP
- HTTP/REST

## Access Policies

Shared access policies:

- Manage
- Send
- Listen

A default `RootManageSharedAccessKey` is created.

## Connection String

    Endpoint=sb://az-203-ddsb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<key>

## Queues

Sending a message (snippet):

```C#
queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

string messageBody = $"Message {i}";
var message = new Message(Encoding.UTF8.GetBytes(messageBody));
await queueClient.SendAsync(message);
```

Receiving a message (snippet):

```C#
queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
{
    MaxConcurrentCalls = 1,
    AutoComplete = false
};

// Register the function that processes messages.
queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
```

Handler function:

```C#
    static async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
        Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
        await queueClient.CompleteAsync(message.SystemProperties.LockToken);
    }
```

## Topics

Send a message (snippet):

```C#
topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

string messageBody = $"Message {i}";
var message = new Message(Encoding.UTF8.GetBytes(messageBody));
await topicClient.SendAsync(message);
```

Subscribe to a topic:

```C#
subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
{
    MaxConcurrentCalls = 1,
    AutoComplete = false
};

// Register the function that processes messages.
subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
```

Handler function:

```C#
static async Task ProcessMessagesAsync(Message message, CancellationToken token)
{
    // Process the message.
    Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

    // Complete the message so that it is not received again.
    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
}
```
