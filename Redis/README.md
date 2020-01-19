# Redis example

## Azure-based example

```bash
ResourceGroup=az-203-training
RedisName=az203-dd
RedisKey=$(az redis list-keys --resource-group $ResourceGroup --name $RedisName --query primaryKey -o tsv)
RedisConnection=$RedisName.redis.cache.windows.net,abortConnect=false,ssl=true,password=$RedisKey

dotnet user-secrets set "CacheConnection" $RedisConnection

dotnet run
```

## Local Docker

    docker run -d --name az203-redis \
        -v $PWD/redis.conf:/usr/local/etc/redis/redis.conf \
        -p 6379:6379 \
        docker.io/redis:5.0-alpine \
        redis-server /usr/local/etc/redis/redis.conf

    # Should return: PONG
    redis-cli PING
