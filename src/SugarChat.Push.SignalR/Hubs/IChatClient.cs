using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(object[] messages, CancellationToken cancellationToken = default);
        Task ReceiveGroupMessage(object[] messages, CancellationToken cancellationToken = default);
        Task ReceiveGlobalMessage(object[] messages, CancellationToken cancellationToken = default);
    }
}
