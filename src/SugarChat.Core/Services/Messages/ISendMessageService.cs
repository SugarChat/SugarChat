using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Command;

namespace SugarChat.Core.Services
{
    public interface ISendMessageService: IService
    {
        Task SendMessage(SendMessageCommand command, CancellationToken cancellationToken);
    }
}
