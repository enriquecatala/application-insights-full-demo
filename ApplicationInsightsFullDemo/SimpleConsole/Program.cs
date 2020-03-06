using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace SimpleConsole
{
    class Program
    {
        private const string _api = "https://smpapienriquetest.azurewebsites.net";
        private static HttpClient _client = new HttpClient(new PollyHandler()) { BaseAddress = new Uri(_api) };

        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            //string response = await _client.GetStringAsync("/api/values/");
            string response = await _client.GetStringAsync("/weatherforecast/");
            Console.WriteLine(response);
        }
    }
}
