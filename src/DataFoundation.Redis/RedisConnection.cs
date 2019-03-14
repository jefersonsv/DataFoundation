using StackExchange.Redis;

namespace DataFoundation.Redis
{
    public class RedisConnection
    {
        public readonly ConnectionMultiplexer Client;
        public readonly IDatabase DB;

        public RedisConnection(string redisConnectionString = null, string password = null)
        {
            redisConnectionString = redisConnectionString ?? System.Environment.GetEnvironmentVariable("REDIS_SERVER") ?? "127.0.0.1";
            password = password ?? System.Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? null;

            ConfigurationOptions options = ConfigurationOptions.Parse(redisConnectionString);
            if (!string.IsNullOrEmpty(password))
                options.Password = password;

            Client = ConnectionMultiplexer.Connect(options);
            DB = Client.GetDatabase();
        }
    }
}