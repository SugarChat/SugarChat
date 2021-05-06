using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger Logger;

        public ChatHub(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<ChatHub>();
        }

        public async Task SendMessage(string userId, object[] messages, CancellationToken cancellationToken = default)
        {
            await Clients.User(userId).SendCoreAsync("message", messages, cancellationToken);
        }

        public async Task SendGroupMessage(string group, object[] messages, CancellationToken cancellationToken = default)
        {
            await Clients.Group(group).SendCoreAsync("groupMessage", messages, cancellationToken);
        }

        public async Task SendAllMessage(object[] messages, CancellationToken cancellationToken = default)
        {
            await Clients.All.SendCoreAsync("allMessage", messages, cancellationToken);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
