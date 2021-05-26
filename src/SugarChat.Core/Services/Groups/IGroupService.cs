using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Events.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroupService : IService
    {
        Task<GroupAddedEvent> AddGroupAsync(AddGroupCommand command, CancellationToken cancellation);
        Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request, CancellationToken cancellation = default);

    }
}