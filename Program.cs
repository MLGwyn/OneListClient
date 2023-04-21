using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleTables;

namespace OneListClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var token = "";

            if (args.Length == 0)
            {
                Console.WriteLine("What List would you like? ");
                token = Console.ReadLine();
            }
            else
            {
                token = args[0];
            }

            var client = new HttpClient();

            var url = $"https://one-list-api.herokuapp.com/items?access_token={token}";
            var responseAsStream = await client.GetStreamAsync(url);

            var items = await JsonSerializer.DeserializeAsync<List<Item>>(responseAsStream);

            var table = new ConsoleTable("Description", "Created At", "Completed");
            // For each item in our deserialized List of Item
            foreach (var item in items)
            {
                // Add one row to our table
                table.AddRow(item.Text, item.CreatedAt, item.CompletedStatus);
            }
            // Write the table
            table.Write();

        }

    }
}
