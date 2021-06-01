using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.SignalR.Client.ConsoleSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:7777");
            var connectionUrl = await httpClient.GetStringAsync("api/chat/GetConnectionUrl?userIdentifier=console");

            HubConnection hubConnection = new HubConnectionBuilder()
              .WithUrl(connectionUrl,
              options => {
                  options.SkipNegotiation = true;
                  options.Transports = HttpTransportType.WebSockets;
              })
              .Build();
            hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await hubConnection.StartAsync();
            };
            hubConnection.On<string,string>("ReceiveMessage", (s1, s2) => 
            {
                Console.WriteLine("UserMessage   " + s1 + ":" + s2);
            });
            hubConnection.On<string,string>("ReceiveGroupMessage", (s1, s2) => 
            {
                Console.WriteLine("GroupMessage   " + s1 + ":" + s2);
            });
            hubConnection.On<string,string>("ReceiveGlobalMessage", (s1, s2) => 
            {
                Console.WriteLine("GlobalMessage   " + s1 + ":" + s2);
            });
            hubConnection.On<string,string>("CustomMessage", (s1, s2) => 
            {
                Console.WriteLine("CustomMessage   " + s1 + ":" + s2);
            });
            await hubConnection.StartAsync();

            Thread.Sleep(int.MaxValue);
        }
    }
}
