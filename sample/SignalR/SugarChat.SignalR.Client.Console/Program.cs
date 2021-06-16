using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message;
using SugarChat.Message.Commands.Messages;

namespace SugarChat.SignalR.Client.ConsoleSample
{
    public class Program
    {
        private static string _id; 
        private static HttpClient _httpClient = new ();
        public static async Task Main(string[] args)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:5000");
            _id = Console.ReadLine();
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
           await _httpClient.GetStringAsync($"api/chat/addToConversations?userId={_id}");

            _httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            while (true)
            {
                Console.WriteLine("Please type in your message");
                string content = Console.ReadLine();
                Console.WriteLine("Please type in the groupId you what to send");
                string groupId = Console.ReadLine();
                SendMessageCommand command = new SendMessageCommand
                    {Content = content, Id = Guid.NewGuid().ToString(), GroupId = groupId, SentBy = _id};
                HttpContent httpContent = new StringContent(JsonSerializer.Serialize(command));
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                await _httpClient.PostAsync("api/message/send",httpContent);
            }
        }
        private static async Task<HubConnection> build()
        {
            var responseString = await _httpClient.GetStringAsync($"api/chat/GetConnectionUrl?userId={_id}");
            
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
