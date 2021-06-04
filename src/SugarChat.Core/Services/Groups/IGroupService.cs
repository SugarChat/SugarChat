using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Events.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Groups;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroupService : IService
    {
        Task<AddGroupEvent> AddGroupAsync(AddGroupCommand command, CancellationToken cancellation = default);
        Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request, CancellationToken cancellation = default);
        Task<RemoveGroupEvent> RemoveGroupAsync(RemoveGroupCommand command, CancellationToken cancellation = default);
        Task<GetGroupProfileResponse> GetGroupProfileAsync(GetGroupProfileRequest request, CancellationToken cancellationToken);
        Task<GroupProfileUpdatedEvent> UpdateGroupProfileAsync(UpdateGroupProfileCommand command, CancellationToken cancellationToken);

    }
}