using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Messages;
using SugarChat.Shared.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Net.Client
{
    public interface ISugarChatClient
    {
        Task<SugarChatResponse> CreateUserAsync(AddUserCommand command, CancellationToken cancellationToken);
        Task<SugarChatResponse<UserDto>> GetUserProfileAsync(GetUserRequest request, CancellationToken cancellationToken);       
        Task<SugarChatResponse> UpdateMyProfileAsync(UpdateUserCommand command, CancellationToken cancellationToken);
        Task<SugarChatResponse> CreateGroupAsync(AddGroupCommand command, CancellationToken cancellationToken);




        Task<SugarChatResponse<int>> GetUnreadMessageCountAsync(GetUnreadMessageCountRequest request, CancellationToken cancellationToken);
    }
}
