using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Users
{
    public interface IUserDataProvider
    {
        Task<User> GetByIdAsync(string id, CancellationToken cancellationToken);
    }
}