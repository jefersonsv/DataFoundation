using System;
using DataFoundation.ElasticSearch;

namespace ElasticSearchConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ElasticConnection("products-error");
            GoAsync(client).Wait();
        }

        private static async System.Threading.Tasks.Task GoAsync(ElasticConnection client)
        {
            var res = await client.DeleteByAsync("asd", "xyz");
        }
    }
}
