using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace redis_app
{

    class RandomNumber
    {
        static readonly Random rand = new Random();

        public int value { get; set; } = rand.Next(1, 100);

    }

    class Program
    {
        private static IConfigurationRoot Configuration { get; set; }
        const string SecretName = "CacheConnection";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Redis!");
            InitializeConfiguration();
            IDatabase cache = lazyConnection.Value.GetDatabase();

            string cacheCommand = "PING";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());

            cacheCommand = "INFO";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());

            // Get the client list, useful to see if connection list is growing...
            cacheCommand = "CLIENT LIST";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : \n" + cache.Execute("CLIENT", "LIST").ToString().Replace("id=", "id="));

            var rando = new RandomNumber();
            var json = JsonSerializer.Serialize(rando);
            cache.StringSet("MyRandomNumber", json);

            var cachedJson = cache.StringGet("MyRandomNumber");
            var cachedValue = JsonSerializer.Deserialize<RandomNumber>(cachedJson);

            Console.WriteLine($"Random number: {rando.value}");
            Console.WriteLine($"Cache response: {cachedJson}");
            Console.WriteLine($"Cache value: {cachedValue.value}");

            lazyConnection.Value.Dispose();
        }

        private static void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();

            Configuration = builder.Build();
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = Configuration[SecretName];
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
