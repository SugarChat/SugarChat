﻿using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using SugarChat.SignalR.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<string> GetConnectionUrl(string userIdentifier, bool isInterior = false)
        {
            var url = await HubConnection.InvokeAsync<string>("GetConnectionUrl", userIdentifier, isInterior).ConfigureAwait(false);
            return url;
        }

        public async Task Group(GroupActionModel model)
        {
            await HubConnection.SendAsync("Group", model).ConfigureAwait(false);
        }

        public async Task SendCustomMessage(SendCustomMessageModel model)
        {
            model.Messages = model.Messages.Select(o => o.ToString()).ToArray();
            await HubConnection.SendAsync("SendCustomMessage", model).ConfigureAwait(false);
        }

        public async Task SendMassCustomMessage(SendMassCustomMessageModel model)
        {
            model.Messages = model.Messages.Select(o => o.ToString()).ToArray();
            await HubConnection.SendAsync("SendMassCustomMessage", model).ConfigureAwait(false);
        }

        public async Task SendMassMessage(SendMassMessageModel model)
        {
            model.Messages = model.Messages.Select(o => o.ToString()).ToArray();
            await HubConnection.SendAsync("SendMassMessage", model).ConfigureAwait(false);
        }

        public async Task SendMessage(SendMessageModel model)
        {
            model.Messages = model.Messages.Select(o => o.ToString()).ToArray();
            await HubConnection.SendAsync("SendMessage", model).ConfigureAwait(false);
        }
    }
}
