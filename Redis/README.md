# Redis example

    docker run -d --name az203-redis \
        -v $PWD/redis.conf:/usr/local/etc/redis/redis.conf \
        -p 6379:6379 \
        docker.io/redis:5.0-alpine \
        redis-server /usr/local/etc/redis/redis.conf

    # Should return: PONG
    redis-cli PING
