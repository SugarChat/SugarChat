using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Users
{
    public interface IUserService
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(string id, CancellationToken cancellationToken);
        Task<User> GetAsync(string id);
        Task AddFriendAsync(string userId, string friendId, CancellationToken cancellationToken);
        Task RemoveFriendAsync(string userId, string friendId);
    }
}