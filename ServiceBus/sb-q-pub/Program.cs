using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace sb_q_pub
{

    class Program
    {

        private const string ENV_CONN_STR = "AZ203_SB_CONNECTIONSTRING";
        private const string ENV_QUEUE = "AZ203_SB_QUEUE";

        private static readonly string ServiceBusConnectionString = Environment.GetEnvironmentVariable(ENV_CONN_STR);
        private static readonly string QueueName = Environment.GetEnvironmentVariable(ENV_QUEUE);
        static IQueueClient queueClient;

        public static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Starting...");

            if (String.IsNullOrEmpty(ServiceBusConnectionString))
            {
                Console.WriteLine($"Please set {ENV_CONN_STR} in your environment");
                return 1;
            }

            if (String.IsNullOrEmpty(QueueName))
            {
                Console.WriteLine($"Please set {ENV_QUEUE} in your environment");
                return 1;
            }

            const int numberOfMessages = 10;
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("======================================================");

            // Send messages.
            await SendMessagesAsync(numberOfMessages);

            Console.ReadKey();

            await queueClient.CloseAsync();

            return 0;
        }

        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the queue.
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the queue.
                    await queueClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
