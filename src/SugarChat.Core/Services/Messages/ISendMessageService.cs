using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Messages.Events;

namespace SugarChat.Core.Services
{
    public interface ISendMessageService : IService
    {
        Task<MessageSentEvent> SendMessageAsync(SendMessageCommand command, CancellationToken cancellationToken = default);
    }
}