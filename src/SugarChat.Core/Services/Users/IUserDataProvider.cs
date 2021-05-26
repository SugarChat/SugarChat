using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Users
{
    public interface IUserDataProvider: IDataProvider
    {
        Task<User> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetRangeByIdAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellation);
        Task UpdateAsync(User user, CancellationToken cancellation);
        Task RemoveAsync(User user, CancellationToken cancellation);
    }
}