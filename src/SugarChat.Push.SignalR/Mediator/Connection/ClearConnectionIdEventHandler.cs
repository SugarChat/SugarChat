using Mediator.Net.Context;
using Mediator.Net.Contracts;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Mediator.Connection
{
    public class ClearConnectionIdEventHandler : IEventHandler<ClearConnectionIdEvent>
    {
        private readonly ICacheService _cache;

        public ClearConnectionIdEventHandler(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task Handle(IReceiveContext<ClearConnectionIdEvent> context, CancellationToken cancellationToken)
        {
            var dic = await _cache.GetHashAll("UserConnectionIds").ConfigureAwait(false);
            var kv = dic.First(kv => kv.Value.Contains(context.Message.ConnectionId));
            var connectionIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(kv.Value);
            connectionIds.Remove(context.Message.ConnectionId);
            await _cache.HashSetAsync("UserConnectionIds", kv.Key, connectionIds).ConfigureAwait(false);
        }
    }
}
