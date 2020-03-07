using ConsoleInterface.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleInterface
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient();

        private static string _urlApiSqlServer = String.Empty;
        private static string _urlApiCosmos = String.Empty;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            
            _urlApiSqlServer = configuration.GetSection("UrlApiSqlServer").Value;
            _urlApiCosmos = configuration.GetSection("UrlApiCosmos").Value;

            /// Get the products
            var products = GetProducts();
            foreach (var item in products.Result)
            {
                Console.WriteLine(item.ProductId + " " + item.Name);
            }

            Console.ReadLine();
        }

        private static async Task<IEnumerable<string>> GetItems(string path)
        {
            var response = await Client.GetAsync(path);

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsAsync<List<string>>();
        }

        private static async Task<List<Product>> GetProducts()
        {
            var users = new List<Product>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_urlApiSqlServer);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/products");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    users = await response.Content.ReadAsAsync<List<Product>>();
                }
            }

            return users;
        }
    }
}
