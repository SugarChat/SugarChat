using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.SignalR.ServerClient.Test
{
    public class ServerHttpClientUnitTest
    {
        [Fact]
        public async Task TestGetConnectionUrl()
        {
            var client = GetClient();
            var urlStr = await client.GetConnectionUrl("Test123");
            Assert.NotNull(urlStr);
        }

        private ServerHttpClient GetClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:9875");
            return new ServerHttpClient(httpClient);
        }
    }
}
