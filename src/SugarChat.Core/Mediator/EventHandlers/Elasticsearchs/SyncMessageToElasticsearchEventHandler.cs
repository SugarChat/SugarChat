using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Elasticsearchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Elasticsearchs
{
    public class SyncMessageToElasticsearchEventHandler : IEventHandler<SyncMessageToElasticsearchEvent>
    {
        public Task Handle(IReceiveContext<SyncMessageToElasticsearchEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
