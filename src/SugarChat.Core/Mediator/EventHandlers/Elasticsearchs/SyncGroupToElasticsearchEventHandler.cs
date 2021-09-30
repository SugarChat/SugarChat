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
    public class SyncGroupToElasticsearchEventHandler : IEventHandler<SyncGroupToElasticsearchEvent>
    {
        public Task Handle(IReceiveContext<SyncGroupToElasticsearchEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
