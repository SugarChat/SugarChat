using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

namespace SugarChat.Core.Services.Friends
{
    public interface IFriendService
    {
        Task<FriendAddedEvent> AddFriendAsync(AddFriendCommand command, CancellationToken cancellation = default);
        Task<FriendRemovedEvent> RemoveFriendAsync(RemoveFriendCommand command, CancellationToken cancellation = default);
    }
}