# Storage Account Queues

* MSFT Docs: https://docs.microsoft.com/en-au/azure/storage/
* Demo: [Storage Account - Queue](Storage\ Account\ -\ Queue/README.md)

Packages:

- `Microsoft.Azure.Storage.Queue`

## Sample code

### Connect

```C#
var connectionString = $"DefaultEndpointsProtocol=https;AccountName={StorageAccountName};AccountKey={StorageAccountKey};EndpointSuffix=core.windows.net";

CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
CloudQueue queue = queueClient.GetQueueReference("pets");
queue.CreateIfNotExists();
```

### Create a queue message

```C#
CloudQueueMessage message = new CloudQueueMessage($"woof {i}");
await queue.AddMessageAsync(message);
```

### Retrieve messages

```C#
CloudQueueMessage retrievedMessage = await queue.GetMessageAsync();
while(retrievedMessage != null) {
    Console.WriteLine($"  - {retrievedMessage.AsString}");
    await queue.DeleteMessageAsync(retrievedMessage);
    retrievedMessage = await queue.GetMessageAsync();
}
```
