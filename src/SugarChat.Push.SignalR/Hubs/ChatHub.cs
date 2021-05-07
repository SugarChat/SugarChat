using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public async Task SendUserMessage([NotNull]string userId, object[] messages, CancellationToken cancellationToken = default)
        {
            await Clients.User(userId).SendCoreAsync("ReceiveMessage", messages, cancellationToken);
        }
        public async Task SendMassUserMessage([NotNull]IReadOnlyList<string> userIds, object[] messages, CancellationToken cancellationToken = default)
        {
            await Clients.Users(userIds).SendCoreAsync("ReceiveMessage", messages, cancellationToken);
        }

        public async Task SendGroupMessage([NotNull]string group, object[] messages, CancellationToken cancellationToken = default)
        {
            await Clients.Group(group).SendCoreAsync("ReceiveGroupMessage", messages, cancellationToken);
        }

        public async Task SendMassGroupMessage([NotNull]IReadOnlyList<string> groups, object[] messages, CancellationToken cancellationToken = default)
        {
            await Clients.Groups(groups).SendCoreAsync("ReceiveGroupMessage", messages, cancellationToken);
        }

        public async Task SendAllMessage(object[] messages, CancellationToken cancellationToken = default)
        {
            await Clients.All.SendCoreAsync("ReceiveGlobalMessage", messages, cancellationToken);
        }

        public async Task CustomMessage(SendWay sendWay, [NotNull]string method, object[] messages, string sendTo = "", CancellationToken cancellationToken = default)
        {
            switch (sendWay)
            {
                case SendWay.User:
                    if (string.IsNullOrWhiteSpace(sendTo)) throw new ArgumentException("The arg {0} is null or whitespace", nameof(sendTo));

                    await Clients.User(sendTo).SendCoreAsync(method, messages, cancellationToken);
                    break;
                case SendWay.Group:
                    if (string.IsNullOrWhiteSpace(sendTo)) throw new ArgumentException("The arg {0} is null or whitespace", nameof(sendTo));

                    await Clients.Group(sendTo).SendCoreAsync(method, messages, cancellationToken);
                    break;
                case SendWay.All:
                    await Clients.All.SendCoreAsync(method, messages, cancellationToken);
                    break;
                default:
                    break;
            }
        }
        public async Task CustomMassMessage(SendWay sendWay, [NotNull]string method, object[] messages, [NotNull]IReadOnlyList<string> sendTo, CancellationToken cancellationToken = default)
        {
            switch (sendWay)
            {
                case SendWay.User:

                    await Clients.Users(sendTo).SendCoreAsync(method, messages, cancellationToken);

                    break;
                case SendWay.Group:

                    await Clients.Groups(sendTo).SendCoreAsync(method, messages, cancellationToken);

                    break;
                default:
                    break;
            }
        }

        public override Task OnConnectedAsync()
        {
            // todo 
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // todo 
            return base.OnDisconnectedAsync(exception);
        }
    }

    
}
