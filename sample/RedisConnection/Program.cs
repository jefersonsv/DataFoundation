﻿using System;

namespace sample
{
    class Program
    {
        static void Main(string[] args)
        {
            DataFoundation.Redis.RedisConnection redis = new DataFoundation.Redis.RedisConnection();
            var somekey = redis.DB.StringGet("somekey");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
