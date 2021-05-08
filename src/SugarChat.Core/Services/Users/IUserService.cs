using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

namespace SugarChat.Core.Services.Users
{
    public interface IUserService
    {
        Task<UserAddedEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellation);
        Task<UserDeletedEvent> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellation);
        Task<FriendAddedEvent> AddFriendAsync(AddFriendCommand command, CancellationToken cancellation);
        Task<FriendRemovedEvent> RemoveFriendAsync(RemoveFriendCommand command, CancellationToken cancellation);
        Task<GetUserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellation);
        Task<GetUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request, CancellationToken cancellation);
    }
}