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
        Task<AddUserEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellation = default);
        Task<UpdateUserEvent> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellation = default);
        Task<UserDeletedEvent> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellation = default);
        Task<GetUserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellation = default);
        Task<GetUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request, CancellationToken cancellation = default);
        Task<GetFriendsOfUserResponse> GetFriendsOfUserAsync(GetFriendsOfUserRequest request, CancellationToken cancellation = default);
    }
}