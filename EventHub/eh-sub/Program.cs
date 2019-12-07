using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace eh_sub
{
    class Program
    {
        private const string ENV_VAR = "AZ203_EH_CONNECTIONSTRING";
        private const string ENV_VAR_SA_KEY = "AZ203_SA_KEY";
        private const string ENV_VAR_SA_NAME = "AZ203_SA_NAME";
        private const string ENV_VAR_SA_CONTAINER = "AZ203_SA_CONTAINER";

        private static EventHubClient eventHubClient;
        private static string EventHubConnectionString = Environment.GetEnvironmentVariable(ENV_VAR);
        private const string EventHubName = "az203";

        private static readonly string StorageContainerName = Environment.GetEnvironmentVariable(ENV_VAR_SA_CONTAINER);
        private static readonly string StorageAccountName = Environment.GetEnvironmentVariable(ENV_VAR_SA_NAME);
        private static readonly string StorageAccountKey = Environment.GetEnvironmentVariable(ENV_VAR_SA_KEY);

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

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

        static int Main(string[] args)
        {
            Console.WriteLine("Starting...");
            if (String.IsNullOrEmpty(EventHubConnectionString))
            {
                Console.WriteLine($"Please set {ENV_VAR} in your environment");
                return 1;
            }

            if (String.IsNullOrEmpty(StorageAccountName))
            {
                Console.WriteLine($"Please set {ENV_VAR_SA_NAME} in your environment");
                return 1;
            }

            if (String.IsNullOrEmpty(StorageAccountKey))
            {
                Console.WriteLine($"Please set {ENV_VAR_SA_KEY} in your environment");
                return 1;
            }

            if (String.IsNullOrEmpty(StorageContainerName))
            {
                Console.WriteLine($"Please set {ENV_VAR_SA_CONTAINER} in your environment");
                return 1;
            }

            MainAsync(args).GetAwaiter().GetResult();
            return 0;

        }
    }
}
