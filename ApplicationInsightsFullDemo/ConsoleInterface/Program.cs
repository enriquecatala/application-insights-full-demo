using ConsoleInterface.Models;
using Microsoft.ApplicationInsights.Extensibility;
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

        static  void Main(string[] args)
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.debug.json", optional: true, reloadOnChange: true);
#else
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#endif

            IConfigurationRoot configuration = builder.Build();


            _urlApiSqlServer = configuration.GetSection("UrlApiSqlServer").Value;
            _urlApiCosmos = configuration.GetSection("UrlApiCosmos").Value;

            #region ApplicationInsights
            TelemetryConfiguration.Active.InstrumentationKey = configuration.GetSection("ApplicationInsights:InstrumentationKey").Value;

#if DEBUG
            // Telemetry results exposed inmediately 
            // Switch it off in production, because it may slow down your app.
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif

            #endregion

            TestCosmosDBInitialization();
            InitializeCosmosDB();

            Console.WriteLine("End of CosmosDB initialization, press a key to continue...");
            Console.ReadLine();
        }

        private static async void InitializeCosmosDB()
        {
            /// Get the products
            var products = GetProducts();
            foreach (var item in products.Result)
            {
                Console.WriteLine(String.Format("Readed from SQL Server: ProductId {0} - {1}", item.ProductId, item.Name));

                //680 is the one used for testing
                if(item.ProductId!=680)
                    await InsertProductInCosmosDb(item);

                Console.WriteLine("Inserted Product in CosmosDB");
            }
        }

        private static async void TestCosmosDBInitialization()
        {
            /// Get the products
            var p = GetProduct(680);

            Console.WriteLine(String.Format("Readed from SQL Server: ProductId {0} - {1}", p.Result.ProductId, p.Result.Name));

            await InsertProductInCosmosDb(p.Result);

            Console.WriteLine("Inserted Product in CosmosDB");
        }

        #region ApiCalls

        private static async Task<List<Product>> GetProducts()
        {
            var retorno = new List<Product>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_urlApiSqlServer);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/products");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    retorno = await response.Content.ReadAsAsync<List<Product>>();
                }
            }

            return retorno;
        }

        private static async Task<Product> GetProduct(int productid)
        {
            var retorno = new Product();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_urlApiSqlServer);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(String.Format("api/products/{0}",productid));
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    retorno = await response.Content.ReadAsAsync<Product>();
                }
            }

            return retorno;
        }

        private static async Task<Product> InsertProductInCosmosDb(Product product)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_urlApiCosmos);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("api/products/postproduct/", product);
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        product = await response.Content.ReadAsAsync<Product>();
                    }
                }

                return product;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        #endregion ApiCalls
    }
}
