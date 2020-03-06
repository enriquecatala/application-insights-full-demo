using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleConsole
{
    public class PollyHandler : DelegatingHandler
    {
        public PollyHandler() : base(new HttpClientHandler()) { }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                .WaitAndRetryForeverAsync(
                    retryAttempt => TimeSpan.FromSeconds(5),
                    (ex, time) => Console.WriteLine("Failed Attempt")
                )
                .ExecuteAsync(() => base.SendAsync(request, cancellationToken));
    }
}
