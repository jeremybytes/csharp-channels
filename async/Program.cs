using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace async
{
    class Program
    {
        public static HttpClient client = new HttpClient() { BaseAddress = new Uri("http://localhost:9874") };
        public static JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public static async Task<List<int>> GetIdsAsync()
        {
            HttpResponseMessage response =
                await client.GetAsync("people/ids").ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Error fetching URL ({client.BaseAddress}people/ids) return status code {response.StatusCode}");
            }
            var stringResult =
                await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<List<int>>(stringResult);
        }

        public static async Task<Person> GetPersonAsync(int id)
        {
            HttpResponseMessage response =
                await client.GetAsync($"people/{id}").ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Error fetching URL ({client.BaseAddress}people/{id}) return status code {response.StatusCode}");
            }
            var stringResult =
                await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<Person>(stringResult, options);
        }

        public record Person(int ID, string GivenName, string FamilyName,
            DateTime StartDate, int Rating, string FormatString)
        {
            public override string ToString()
            {
                if (string.IsNullOrEmpty(FormatString))
                    return $"{GivenName} {FamilyName}";
                return string.Format(FormatString, GivenName, FamilyName);
            }
        }

        static async Task Main(string[] args)
        {
            try
            {
                var start = DateTime.Now;
                var ids = await GetIdsAsync();
                Console.WriteLine(ids.ToDelimitedString(" "));

                var channel = Channel.CreateBounded<Person>(10);
                var asyncCalls = new List<Task>();

                // Write person objects to channel asynchronously
                foreach (var id in ids)
                {
                    async Task func(int id)
                    {
                        try
                        {
                            var person = await GetPersonAsync(id);
                            await channel.Writer.WriteAsync(person);
                        }
                        catch (HttpRequestException ex)
                        {
                            Console.WriteLine($"ERROR: ID {id}: {ex.Message}");
                        }
                    }

                    asyncCalls.Add(func(id));
                }

                // Wait for the async tasks to complete & close the channel
                _ = Task.Run(async () =>
                    {
                        await Task.WhenAll(asyncCalls);
                        channel.Writer.Complete();
                    });

                // Read person objects from the channel until the channel is closed
                // OPTION 1: Using ReadAllAsync (IAsyncEnumerable)
                await foreach (Person person in channel.Reader.ReadAllAsync())
                {
                    Console.WriteLine($"{person.ID}: {person}");
                }

                // Read person objects from the channel until the channel is closed
                // OPTION 2: Using WaitToReadAsync / TryRead
                // while (await channel.Reader.WaitToReadAsync())
                // {
                //    while (channel.Reader.TryRead(out Person person))
                //    {
                //        Console.WriteLine($"{person.ID}: {person}");
                //    }
                // }

                var elapsed = DateTime.Now - start;
                Console.WriteLine($"Total Time: {elapsed}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}
