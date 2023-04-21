using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneListClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();

            var responseAsStream = await client.GetStreamAsync("https://one-list-api.herokuapp.com/items?access_token=melissa-gwyn");

            var items = await JsonSerializer.DeserializeAsync<List<Item>>(responseAsStream);

            foreach (var item in items)
            {
                // Output some details on that item
                Console.WriteLine($"The task {item.Text} was created on {item.Created_at} and is {item.CompletedStatus}");
            }

        }
    }
}
