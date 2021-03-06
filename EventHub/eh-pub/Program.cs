﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace eh_pub
{
    class Program
    {

        private const string ENV_VAR = "AZ203_EH_CONNECTIONSTRING";

        private static EventHubClient eventHubClient;
        private static readonly string EventHubConnectionString = Environment.GetEnvironmentVariable(ENV_VAR);
        private const string EventHubName = "az203";

        private static async Task MainAsync(string[] args)
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but this simple scenario
            // uses the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(100);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        // Uses the event hub client to send 100 messages to the event hub.
        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    var message = $"Message {i}";
                    Console.WriteLine($"Sending message: {message}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            if (String.IsNullOrEmpty(EventHubConnectionString))
            {
                Console.WriteLine($"Please set {ENV_VAR} in your environment");
            }
            else
            {
                MainAsync(args).GetAwaiter().GetResult();
            }
        }

    }
}
