using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public interface IChatHubService
    {
        Task SendUserMessage([NotNull] string userId, string[] messages, CancellationToken cancellationToken = default);
        Task SendMassUserMessage([NotNull] IReadOnlyList<string> userIds, string[] messages, CancellationToken cancellationToken = default);

        Task SendGroupMessage([NotNull] string group, string[] messages, CancellationToken cancellationToken = default);
        Task SendMassGroupMessage([NotNull] IReadOnlyList<string> groups, string[] messages, CancellationToken cancellationToken = default);

        Task SendAllMessage(string[] messages, CancellationToken cancellationToken = default);

        Task CustomMessage(SendWay sendWay, [NotNull] string method, string[] messages, string sendTo = "", CancellationToken cancellationToken = default);
        Task CustomMassMessage(SendWay sendWay, [NotNull] string method, string[] messages, [NotNull] IReadOnlyList<string> sendTo, CancellationToken cancellationToken = default);

        Task AddGroup([NotNull] string connectionId, [NotNull] string groupName, CancellationToken cancellationToken = default);
        Task AddGroups([NotNull] string connectionId, [NotNull] IReadOnlyList<string> groupNames, CancellationToken cancellationToken = default);
        Task ExitGroup([NotNull] string connectionId, [NotNull] string groupName, CancellationToken cancellationToken = default);
        Task ExitGroups([NotNull] string connectionId, [NotNull] IReadOnlyList<string> groupNames, CancellationToken cancellationToken = default);
    }
}
