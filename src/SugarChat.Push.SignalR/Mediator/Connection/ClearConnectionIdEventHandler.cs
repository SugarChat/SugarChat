using Mediator.Net.Context;
using Mediator.Net.Contracts;
using ServiceStack.Redis;
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
        private readonly IRedisClient _redis;

        public ClearConnectionIdEventHandler(IRedisClient redis)
        {
            _redis = redis;
        }

        public async Task Handle(IReceiveContext<ClearConnectionIdEvent> context, CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() =>
            {
                var dic = _redis.GetAllEntriesFromHash("UserConnectionIds");
                var kv = dic.First(kv => kv.Value.Contains(context.Message.ConnectionId));
                var connectionIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(kv.Value);
                connectionIds.Remove(context.Message.ConnectionId);
                _redis.SetEntryInHash("UserConnectionIds", kv.Key, System.Text.Json.JsonSerializer.Serialize(connectionIds));
            }, cancellationToken);
        }
    }
}
