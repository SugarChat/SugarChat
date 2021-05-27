using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands;

namespace SugarChat.Core.Services
{
    public interface ISendMessageService: IService
    {
        Task SendMessage(SendMessageCommand command, CancellationToken cancellationToken);
    }
}
