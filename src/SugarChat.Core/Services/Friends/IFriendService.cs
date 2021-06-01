using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

namespace SugarChat.Core.Services.Friends
{
    public interface IFriendService : IService
    {
        Task<AddFriendEvent> AddFriendAsync(AddFriendCommand command, CancellationToken cancellation = default);
        Task<RemoveFriendEvent> RemoveFriendAsync(RemoveFriendCommand command, CancellationToken cancellation = default);
    }
}