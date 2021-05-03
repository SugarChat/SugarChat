using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Users
{
    public interface IUserService
    {
        void Add(User user);
        Task AddAsync(User user, CancellationToken cancellationToken);
        void Delete(string id);
        Task DeleteAsync(string id, CancellationToken cancellationToken);
        User Get(string id);
        Task<User> GetAsync(string id);
        void AddFriend(string userId, string friendId);
        Task AddFriendAsync(string userId, string friendId, CancellationToken cancellationToken);
        void RemoveFriend(string userId, string friendId);
        Task RemoveFriendAsync(string userId, string friendId);
    }
}