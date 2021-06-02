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
        Task<AddUserEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellation = default);
        Task<UpdateUserEvent> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellation = default);
        Task<RemoveUserEvent> RemoveUserAsync(RemoveUserCommand command, CancellationToken cancellation = default);
        Task<GetUserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellation = default);
        Task<GetCurrentUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request, CancellationToken cancellation = default);
        Task<GetFriendsOfUserResponse> GetFriendsOfUserAsync(GetFriendsOfUserRequest request, CancellationToken cancellation = default);
    }
}