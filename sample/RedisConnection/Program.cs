using System;

namespace sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var redis = new DataFoundation.Redis.RedisConnection();
            var somekey = redis.DB.StringGet("https:www:google:com");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
