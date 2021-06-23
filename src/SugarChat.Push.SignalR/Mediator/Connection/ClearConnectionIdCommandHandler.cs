﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Cache;
using SugarChat.Push.SignalR.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Mediator.Connection
{
    public class ClearConnectionIdCommandHandler : ICommandHandler<ClearConnectionIdCommand>
    {
        private readonly ICacheService _cache;

        public ClearConnectionIdCommandHandler(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task Handle(IReceiveContext<ClearConnectionIdCommand> context, CancellationToken cancellationToken)
        {
            var dic = await _cache.GetHashAll(CacheKey.UserConnectionIds).ConfigureAwait(false);
            var kv = dic.First(kv => kv.Value.Contains(context.Message.ConnectionId));
            var connectionIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(kv.Value);
            connectionIds.Remove(context.Message.ConnectionId);
            await _cache.HashSetAsync(CacheKey.UserConnectionIds, kv.Key, connectionIds).ConfigureAwait(false);
        }
    }
}