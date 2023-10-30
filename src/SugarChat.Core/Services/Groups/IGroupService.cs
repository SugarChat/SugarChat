using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Events.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Groups;
using SugarChat.Message.Dtos;
using static SugarChat.Core.Services.Groups.GroupService;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroupService : IService
    {
        Task<GroupAddedEvent> AddGroupAsync(AddGroupCommand command, CancellationToken cancellation = default);

        Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request,
            CancellationToken cancellation = default);

        Task<GroupRemovedEvent> RemoveGroupAsync(RemoveGroupCommand command, CancellationToken cancellation = default);

        Task<GetGroupProfileResponse> GetGroupProfileAsync(GetGroupProfileRequest request,
            CancellationToken cancellationToken = default);

        Task<GroupProfileUpdatedEvent> UpdateGroupProfileAsync(UpdateGroupProfileCommand command,
            CancellationToken cancellationToken = default);

        Task<GroupDismissedEvent> DismissGroupAsync(DismissGroupCommand command,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupDto>> GetByCustomProperties(GetGroupByCustomPropertiesRequest request, CancellationToken cancellationToken);

        Task MigrateCustomPropertyAsync(CancellationToken cancellation = default);

        Task BatchAddGroupAsync(BatchAddGroupCommand command, CancellationToken cancellation = default);
    }
}