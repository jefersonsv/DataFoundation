using StackExchange.Redis;

namespace DataFoundation.Redis
{
    public class RedisConnection
    {
        private ConnectionMultiplexer redis;

        public RedisConnection(string redisConnectionString = null, string password = null)
        {
            if (string.IsNullOrEmpty(redisConnectionString))
                redisConnectionString = "127.0.0.1:6379";

            ConfigurationOptions options = ConfigurationOptions.Parse(redisConnectionString);
            if (!string.IsNullOrEmpty(password))
                options.Password = password;
            else
            {
                // check for environment variables
                var env = System.Environment.GetEnvironmentVariable("REDIS_PASSWORD");
                if (!string.IsNullOrEmpty(env))
                {
                    options.Password = env;
                }
            }

            redis = ConnectionMultiplexer.Connect(options);
            DB = redis.GetDatabase();
        }

        public IDatabase DB { get; private set; }
    }
}