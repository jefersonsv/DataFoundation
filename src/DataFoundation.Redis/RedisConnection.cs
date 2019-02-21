using StackExchange.Redis;

namespace DataFoundation.Redis
{
    public class RedisConnection
    {
        private ConnectionMultiplexer redis;

        public RedisConnection(string redisConnectionString)
        {
            if (string.IsNullOrEmpty(redisConnectionString))
                redisConnectionString = "127.0.0.1:6379";

            redis = ConnectionMultiplexer.Connect(redisConnectionString);
            DB = redis.GetDatabase();
        }

        public IDatabase DB { get; private set; }
    }
}