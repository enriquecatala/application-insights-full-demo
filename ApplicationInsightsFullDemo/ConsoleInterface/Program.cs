using ConsoleInterface.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.DependencyCollector;
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

        private static TelemetryClient _telemetryClient = null;


        private static string _urlApiSqlServer = String.Empty;
        private static string _urlApiCosmos = String.Empty;

        static void Main(string[] args)
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


            InitTelemetry(configuration.GetSection("ApplicationInsights:InstrumentationKey").Value);

            using (var operation = _telemetryClient.StartOperation<RequestTelemetry>("TestCosmosDBInitialization"))
            {
                TestCosmosDBInitialization();
                _telemetryClient.StopOperation(operation); //flush
            }
            using (var operation = _telemetryClient.StartOperation<RequestTelemetry>("InitializeCosmosDB"))
            {
                InitializeCosmosDB();
                _telemetryClient.StopOperation(operation); //flush
            }

            Console.WriteLine("End of CosmosDB initialization, press a key to continue...");
            Console.ReadLine();
        }

        private static async void InitializeCosmosDB()
        {
            Task<List<Product>> products = null;
            using (var operation = _telemetryClient.StartOperation<RequestTelemetry>("InsertProductInCosmosDb"))
            {
                /// Get the products
                products = GetProducts();
                _telemetryClient.StopOperation(operation); //flush
            }

            foreach (var item in products.Result)
            {
                Console.WriteLine(String.Format("Readed from SQL Server: ProductId {0} - {1}", item.ProductId, item.Name));

                //680 is the one used for testing
                if (item.ProductId != 680)
                    using (var operation = _telemetryClient.StartOperation<RequestTelemetry>("InsertProductInCosmosDb"))
                    {
                        await InsertProductInCosmosDb(item);
                        _telemetryClient.StopOperation(operation); //flush

                        Console.WriteLine(String.Format("Inserted Product {0} in CosmosDB", item.ProductId));
                    }
            }
        }

        private static async void TestCosmosDBInitialization()
        {
            /// Get the products
            var p = GetProduct(680);

            Console.WriteLine(String.Format("Readed from SQL Server: ProductId {0} - {1}", p.Result.ProductId, p.Result.Name));

            await InsertProductInCosmosDb(p.Result);

            Console.WriteLine(String.Format("Inserted Product {0} in CosmosDB", p.Result.ProductId));
        }

        #region ApplicationInsights

        public static void InitTelemetry(string key)
        {

            TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = key;
            telemetryConfiguration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            /// Initialization of dependency tracking
            DependencyTrackingTelemetryModule depModule = new DependencyTrackingTelemetryModule();
            depModule.Initialize(telemetryConfiguration);

            _telemetryClient = new TelemetryClient(telemetryConfiguration);

           // TelemetryConfiguration.Active.InstrumentationKey = key;

            //_telemetryClient.InstrumentationKey = key;
            _telemetryClient.Context.User.Id = Environment.UserName;
            _telemetryClient.Context.Operation.Id = Guid.NewGuid().ToString();
            _telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            _telemetryClient.Context.Cloud.RoleName = "ConsoleInterface";


            /// Custom properites
            var properties = new Dictionary<string, string>
            {
#if DEBUG
                {"DEBUG_MODE",   "true"},
#endif
                {"Version", String.Format("v{0} ", typeof(ConsoleInterface.Program).Assembly.GetName().Version) }
            };
            foreach (var p in properties)
            {
                if (!_telemetryClient.Context.Properties.ContainsKey(p.Key))
                    _telemetryClient.Context.Properties.Add(p);
            }
#if DEBUG
            // Telemetry results exposed inmediately 
            // Switch it off in production, because it may slow down your app.
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif

     
            
        }

        #endregion 

        #region ApiCalls

        private static async Task<List<Product>> GetProducts()
        {
            var retorno = new List<Product>();

            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            bool success = false;
            
            try
            {
                // making dependency call
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
                        success = true;
                    }
                }
            }
            finally
            {
                timer.Stop();
                _telemetryClient.TrackDependency("HTTP", "WebApiSqlServer", "api/products/", startTime, timer.Elapsed, success);
            }

            return retorno;
        }

        private static async Task<Product> GetProduct(int productid)
        {
            var retorno = new Product();

            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            bool success = false;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_urlApiSqlServer);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(String.Format("api/products/{0}", productid));
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        retorno = await response.Content.ReadAsAsync<Product>();
                        success = true;
                    }
                }
            }
            finally
            {
                timer.Stop();
                _telemetryClient.TrackDependency("HTTP", "WebApiSqlServer", String.Format("api/products/{0}", productid), startTime, timer.Elapsed, success);
            }
            return retorno;
        }

        private static async Task<Product> InsertProductInCosmosDb(Product product)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var timer = System.Diagnostics.Stopwatch.StartNew();
                bool success = false;
                int pid = product.ProductId;
                try
                {
                    // making dependency call
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
                            success = true;
                        }
                    }
                }
                finally
                {
                    timer.Stop();
                    _telemetryClient.TrackDependency("HTTP", "WebApiCosmosDb", String.Format("api/products/postproduct/", pid.ToString()), startTime, timer.Elapsed, success);
                }
                return product;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion ApiCalls
    }
}
