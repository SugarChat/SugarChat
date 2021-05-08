using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Events.Groups;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroupService
    {
        Task<GroupAddedEvent> AddGroupAsync(AddGroupCommand command, CancellationToken cancellation);
    }
}