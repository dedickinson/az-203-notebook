namespace QueueClientApp
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Queue;


    class Program
    {
        private const string ENV_VAR_SA_KEY = "AZ203_SA_KEY";
        private const string ENV_VAR_SA_NAME = "AZ203_SA_NAME";

        private static readonly string StorageAccountName = Environment.GetEnvironmentVariable(ENV_VAR_SA_NAME);
        private static readonly string StorageAccountKey = Environment.GetEnvironmentVariable(ENV_VAR_SA_KEY);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");

            Console.WriteLine($"Storage account: {StorageAccountName}");

            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={StorageAccountName};AccountKey={StorageAccountKey};EndpointSuffix=core.windows.net";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("pets");
            queue.CreateIfNotExists();

            Console.WriteLine("Sending messages");
            for (var i=1; i<=10; i++) {
                CloudQueueMessage message = new CloudQueueMessage($"woof {i}");
                await queue.AddMessageAsync(message);
            }

            Console.WriteLine("Receiving massages");
            CloudQueueMessage retrievedMessage = await queue.GetMessageAsync();
            while(retrievedMessage != null) {
                Console.WriteLine($"  - {retrievedMessage.AsString}");
                await queue.DeleteMessageAsync(retrievedMessage);
                retrievedMessage = await queue.GetMessageAsync();
            }
        }
    }
}
