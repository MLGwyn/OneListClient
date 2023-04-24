using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleTables;

namespace OneListClient
{
    class Program
    {
        static async Task DeleteOneItem(string token, int id)
        {
            try
            {
                var client = new HttpClient();
                var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";
                var response = await client.DeleteAsync(url);
                await client.DeleteAsync(url);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("I could not find that item!");
            }
        }

        static async Task UpdateOneItem(string token, int id, Item updatedItem)
        {
            var client = new HttpClient();

            var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";
            var jsonBody = JsonSerializer.Serialize(updatedItem);
            var jsonBodyAsContent = new StringContent(jsonBody);
            jsonBodyAsContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(url, jsonBodyAsContent);
            var responseJson = await response.Content.ReadAsStreamAsync();
            var item = await JsonSerializer.DeserializeAsync<Item>(responseJson);
            var table = new ConsoleTable("ID", "Description", "Created At", "Updated At", "Completed");

            table.AddRow(item.Id, item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);
            table.Write(Format.Minimal);
        }

        static async Task AddOneItem(string token, Item newItem)
        {
            var client = new HttpClient();
            var url = $"https://one-list-api.herokupp.com/items?access_token={token}";
            var jsonBody = JsonSerializer.Serialize(newItem);
            var jsonBodyAsContent = new StringContent(jsonBody);
            jsonBodyAsContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url, jsonBodyAsContent);
            var responseJson = await response.Content.ReadAsStreamAsync();
            var item = await JsonSerializer.DeserializeAsync<Item>(responseJson);
            var table = new ConsoleTable("ID", "Description", "Created At", "Updated At", "Completed");

            table.AddRow(item.Id, item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);
            table.Write(Format.Minimal);

        }
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

            var table = new ConsoleTable("ID", "Description", "Created At", "Completed");
            // For each item in our deserialized List of Item
            foreach (var item in items)
            {
                // Add one row to our table
                table.AddRow(item.Id, item.Text, item.CreatedAt, item.CompletedStatus);
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
                Console.Write("Get (A)ll todo, or Get (O)ne todo, (C)reate a new item, (U)pdate an item, (D)elete an item,  or (Q)uit: ");
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

                    case "C":
                        Console.Write("Enter the description of your new todo: ");
                        var text = Console.ReadLine();

                        var newItem = new Item
                        {
                            Text = text
                        };

                        await AddOneItem(token, newItem);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    case "U":
                        Console.Write("Enter the ID of the item tp update: ");
                        var existingId = int.Parse(Console.ReadLine());

                        Console.Write("Enter the new description: ");
                        var newText = Console.ReadLine();

                        Console.Write("Enter yes or no to indicate if the item is complete: ");
                        var newComplete = Console.ReadLine().ToLower() == "yes";

                        var updatedItem = new Item
                        {
                            Text = newText,
                            Complete = newComplete
                        };

                        await UpdateOneItem(token, existingId, updatedItem);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    case "D":
                        Console.Write("Enter the ID of the item to delete: ");
                        var idToDelete = int.Parse(Console.ReadLine());

                        await DeleteOneItem(token, idToDelete);

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
