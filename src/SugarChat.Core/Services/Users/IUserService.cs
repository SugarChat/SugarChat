using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

namespace SugarChat.Core.Services.Users
{
    public interface IUserService : IService
    {
        Task<UserAddedEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellationToken = default);
        Task<UserUpdatedEvent> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken = default);
        Task<UserRemovedEvent> RemoveUserAsync(RemoveUserCommand command, CancellationToken cancellationToken = default);
        Task RemoveAllUserAsync(CancellationToken cancellationToken = default);
        Task<GetUserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken = default);

        Task<GetCurrentUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request,
            CancellationToken cancellationToken = default);

        Task<GetFriendsOfUserResponse> GetFriendsOfUserAsync(GetFriendsOfUserRequest request,
            CancellationToken cancellationToken = default);

        Task<UsersBatchAddedEvent> BatchAddUsersAsync(BatchAddUsersCommand command, CancellationToken cancellationToken = default);

        Task<bool> CheckUserExistAsync(CheckUserExistCommand command, CancellationToken cancellationToken = default);
    }
}