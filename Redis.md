# Redis

* MSFT Docs: https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/
* Demo: [Redis](Redis)

Packages:

- StackExchange.Redis

## Connection String

    <cache name>.redis.cache.windows.net,abortConnect=false,ssl=true,password=<primary-access-key>

## Example code

Connect:

```C#
using StackExchange.Redis;

private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
{
    string cacheConnection = Configuration[SecretName];
    return ConnectionMultiplexer.Connect(cacheConnection);
});

IDatabase cache = lazyConnection.Value.GetDatabase();
```

Check a connection:

```C#
string cacheCommand = "PING";
Console.WriteLine("\nCache command  : " + cacheCommand);
Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());

cacheCommand = "INFO";
Console.WriteLine("\nCache command  : " + cacheCommand);
Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());
```

Add an item:

```C#
var rando = new RandomNumber();
var json = JsonSerializer.Serialize(rando);
cache.StringSet("MyRandomNumber", json);
```

Retrieve an item:

```C#
var cachedJson = cache.StringGet("MyRandomNumber");
var cachedValue = JsonSerializer.Deserialize<RandomNumber>(cachedJson);
```

```C#
lazyConnection.Value.Dispose();
```
