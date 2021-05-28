using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using SugarChat.SignalR.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.SignalR.ServerClient
{
    public class ServerWebScoketClient : IServerClient
    {
        private HubConnection HubConnection;

        public ServerWebScoketClient(string connectionUrl)
        {
            HubConnection = new HubConnectionBuilder()
              .WithUrl(connectionUrl,
              options => {
                  options.SkipNegotiation = true;
                  options.Transports = HttpTransportType.WebSockets;
              })
              .Build();
            HubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await HubConnection.StartAsync();
            };
            HubConnection.StartAsync();
        }

        public async Task<string> GetConnectionUrl(string userIdentifier)
        {
            var url = await HubConnection.InvokeAsync<string>("GetConnectionUrl", userIdentifier).ConfigureAwait(false);
            return url;
        }

        public async Task Group(GroupActionModel model)
        {
            await HubConnection.SendAsync("Group", model).ConfigureAwait(false);
        }

        public async Task SendCustomMessage(SendCustomMessageModel model)
        {
            await HubConnection.SendAsync("SendCustomMessage", model).ConfigureAwait(false);
        }

        public async Task SendMassCustomMessage(SendMassCustomMessageModel model)
        {
            await HubConnection.SendAsync("SendMassCustomMessage", model).ConfigureAwait(false);
        }

        public async Task SendMassMessage(SendMassMessageModel model)
        {
            await HubConnection.SendAsync("SendMassMessage", model).ConfigureAwait(false);
        }

        public async Task SendMessage(SendMessageModel model)
        {
            await HubConnection.SendAsync("SendMessage", model).ConfigureAwait(false);
        }
    }
}
