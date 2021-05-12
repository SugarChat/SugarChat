using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public interface IChatHubService
    {
        Task SendUserMessage([NotNull] string userId, object[] messages, CancellationToken cancellationToken = default);
        Task SendMassUserMessage([NotNull] IReadOnlyList<string> userIds, object[] messages, CancellationToken cancellationToken = default);

        Task SendGroupMessage([NotNull] string group, object[] messages, CancellationToken cancellationToken = default);
        Task SendMassGroupMessage([NotNull] IReadOnlyList<string> groups, object[] messages, CancellationToken cancellationToken = default);

        Task SendAllMessage(object[] messages, CancellationToken cancellationToken = default);

        Task CustomMessage(SendWay sendWay, [NotNull] string method, object[] messages, string sendTo = "", CancellationToken cancellationToken = default);
        Task CustomMassMessage(SendWay sendWay, [NotNull] string method, object[] messages, [NotNull] IReadOnlyList<string> sendTo, CancellationToken cancellationToken = default);

        Task AddGroup([NotNull] string connectionId, [NotNull] string groupName, CancellationToken cancellationToken = default);
        Task ExitGroup([NotNull] string connectionId, [NotNull] string groupName, CancellationToken cancellationToken = default);
    }
}
