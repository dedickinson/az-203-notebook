# Redis

* MSFT Docs: https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/
* Demo: <Redis/README.md>

Packages:

- StackExchange.Redis

## Connection String

    <cache name>.redis.cache.windows.net,abortConnect=false,ssl=true,password=<primary-access-key>

    az203-dd.redis.cache.windows.net:6380,ssl=True,abortConnect=False,password=<KEY>

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
