using Microsoft.AspNetCore.SignalR;
using SugarChat.Push.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatHubService(IHubContext<ChatHub> chatHubContext)
        {
            _chatHubContext = chatHubContext;
        }

        public async Task SendUserMessage([NotNull] string userId, string[] messages, CancellationToken cancellationToken = default)
        {
            await _chatHubContext.Clients.User(userId).SendCoreAsync("ReceiveMessage", messages, cancellationToken).ConfigureAwait(false);
        }
        public async Task SendMassUserMessage([NotNull] IReadOnlyList<string> userIds, string[] messages, CancellationToken cancellationToken = default)
        {
            await _chatHubContext.Clients.Users(userIds).SendCoreAsync("ReceiveMessage", messages, cancellationToken).ConfigureAwait(false);
        }

        public async Task SendGroupMessage([NotNull] string group, string[] messages, CancellationToken cancellationToken = default)
        {
            await _chatHubContext.Clients.Group(group).SendCoreAsync("ReceiveGroupMessage", messages, cancellationToken).ConfigureAwait(false);
        }

        public async Task SendMassGroupMessage([NotNull] IReadOnlyList<string> groups, string[] messages, CancellationToken cancellationToken = default)
        {
            await _chatHubContext.Clients.Groups(groups).SendCoreAsync("ReceiveGroupMessage", messages, cancellationToken).ConfigureAwait(false);
        }

        public async Task SendAllMessage(string[] messages, CancellationToken cancellationToken = default)
        {
            await _chatHubContext.Clients.All.SendCoreAsync("ReceiveGlobalMessage", messages, cancellationToken).ConfigureAwait(false);
        }

        public async Task CustomMessage(SendWay sendWay, [NotNull] string method, string[] messages, string sendTo = "", CancellationToken cancellationToken = default)
        {
            switch (sendWay)
            {
                case SendWay.User:
                    if (string.IsNullOrWhiteSpace(sendTo)) throw new ArgumentException("The arg {0} is null or whitespace", nameof(sendTo));

                    await _chatHubContext.Clients.User(sendTo).SendCoreAsync(method, messages, cancellationToken).ConfigureAwait(false);
                    break;
                case SendWay.Group:
                    if (string.IsNullOrWhiteSpace(sendTo)) throw new ArgumentException("The arg {0} is null or whitespace", nameof(sendTo));

                    await _chatHubContext.Clients.Group(sendTo).SendCoreAsync(method, messages, cancellationToken).ConfigureAwait(false);
                    break;
                case SendWay.All:
                    await _chatHubContext.Clients.All.SendCoreAsync(method, messages, cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }
        public async Task CustomMassMessage(SendWay sendWay, [NotNull] string method, string[] messages, [NotNull] IReadOnlyList<string> sendTo, CancellationToken cancellationToken = default)
        {
            switch (sendWay)
            {
                case SendWay.User:

                    await _chatHubContext.Clients.Users(sendTo).SendCoreAsync(method, messages, cancellationToken).ConfigureAwait(false);

                    break;
                case SendWay.Group:

                    await _chatHubContext.Clients.Groups(sendTo).SendCoreAsync(method, messages, cancellationToken).ConfigureAwait(false);

                    break;
                default:
                    break;
            }
        }

        public async Task AddGroup(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            await _chatHubContext.Groups.AddToGroupAsync(connectionId, groupName, cancellationToken).ConfigureAwait(false);
        }
        public async Task ExitGroup(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            await _chatHubContext.Groups.RemoveFromGroupAsync(connectionId, groupName, cancellationToken).ConfigureAwait(false);
        }
    }
}
