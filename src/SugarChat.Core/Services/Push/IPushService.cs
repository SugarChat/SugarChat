using Mediator.Net.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Push
{
    /// <summary>
    /// use to notification or signalr
    /// </summary>
    public interface IPushService : IService
    {
        Task Push(IEvent @event, CancellationToken cancellationToken);
    }
}
