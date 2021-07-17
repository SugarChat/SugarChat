using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Users
{
    public interface IUserDataProvider : IDataProvider
    {
        Task<User> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetRangeByIdAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task RemoveAsync(User user, CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
    }
}