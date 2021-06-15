using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.SignalR.Client.ConsoleSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            HubConnection hubConnection = await build();
            var count = 1;
            hubConnection.Closed += async (error) =>
            {
                if (count > 5)
                {
                    hubConnection = await build();
                    count = 0;
                }
                count++;
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
        private static async Task<HubConnection> build()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");
            var responseString = await httpClient.GetStringAsync("api/chat/GetConnectionUrl?userId=1");
            
            SugarChatResponse<string> response = 
                JsonSerializer.Deserialize<SugarChatResponse<string>>(responseString,new JsonSerializerOptions{PropertyNameCaseInsensitive = true});

            if (response.Code!= 20000)
            {
                throw new Exception(response.Message);
            }

            HubConnection hubConnection = new HubConnectionBuilder()
              .WithUrl(response.Data,
              options => {
                  options.SkipNegotiation = true;
                  options.Transports = HttpTransportType.WebSockets;
              })
              .Build();
            return hubConnection;
        }
    }
}
