﻿using Microsoft.AspNetCore.SignalR;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IRedisClient _redis;

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

        public async Task AddGroup([NotNull] string connectionId, [NotNull] string groupName, CancellationToken cancellationToken = default)
        {
            try
            {
                await _chatHubContext.Groups.AddToGroupAsync(connectionId, groupName, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                _redis.ScanAllHashEntries("UserConnectionIds", "*" + connectionId + "*");
                var dic = _redis.GetAllEntriesFromHash("UserConnectionIds");
                foreach (var kv in dic)
                {
                    if (kv.Value.Contains(connectionId))
                    {
                        var connectionIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(kv.Value);
                        connectionIds.Remove(connectionId);
                        _redis.SetEntryInHash("UserConnectionIds", kv.Key, System.Text.Json.JsonSerializer.Serialize(connectionIds));
                    }
                }
            }
        }
        public async Task AddGroups([NotNull] string connectionId, [NotNull] IReadOnlyList<string> groupNames, CancellationToken cancellationToken = default)
        {
            foreach (var groupName in groupNames)
            {
                await AddGroup(connectionId, groupName, cancellationToken).ConfigureAwait(false);
            }
        }
        public async Task ExitGroup([NotNull] string connectionId, [NotNull] string groupName, CancellationToken cancellationToken = default)
        {
            try
            {
                await _chatHubContext.Groups.RemoveFromGroupAsync(connectionId, groupName, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                var dic = _redis.GetAllEntriesFromHash("UserConnectionIds");
                foreach (var kv in dic)
                {
                    if (kv.Value.Contains(connectionId))
                    {
                        var connectionIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(kv.Value);
                        connectionIds.Remove(connectionId);
                        _redis.SetEntryInHash("UserConnectionIds", kv.Key, System.Text.Json.JsonSerializer.Serialize(connectionIds));
                    }
                }
            }
        }

        public async Task ExitGroups([NotNull] string connectionId, [NotNull] IReadOnlyList<string> groupNames, CancellationToken cancellationToken = default)
        {
            foreach (var groupName in groupNames)
            {
                await ExitGroup(connectionId, groupName, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
