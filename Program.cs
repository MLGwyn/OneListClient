﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleTables;

namespace OneListClient
{
    class Program
    {
        static async Task GetOneItem(string token, int id)
        {
            try
            {
                var client = new HttpClient();
                // Generate a URL specifically referencing the endpoint for getting a single
                // todo item and provide the id we were supplied
                var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";
                var responseAsStream = await client.GetStreamAsync(url);
                // Supply that *stream of data* to a Deserialize that will interpret it as a *SINGLE* `Item`
                var item = await JsonSerializer.DeserializeAsync<Item>(responseAsStream);
                var table = new ConsoleTable("ID", "Description", "Created At", "Updated At", "Completed");
                // Add one row to our table
                table.AddRow(item.Id, item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);
                // Write the table
                table.Write(Format.Minimal);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("I could not find that item. ");
            }
        }

        static async Task ShowAllItems(string token)
        {
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
            table.Write(Format.Minimal);
        }

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

            var keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                Console.WriteLine("Get (A)ll todo, (O)ne todo, or (Q)uit: ");
                var choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "Q":
                        keepGoing = false;
                        break;

                    case "O":
                        Console.Write("Enter the ID of the item to show: ");
                        var id = int.Parse(Console.ReadLine());
                        await GetOneItem(token, id);
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    case "A":
                        await ShowAllItems(token);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    default:
                        break;

                }
            }
        }
    }
}
