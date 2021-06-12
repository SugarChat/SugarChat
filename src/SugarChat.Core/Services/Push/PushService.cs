using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Push
{
    public class PushService : IPushService
    {
        public Task Push(IEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
