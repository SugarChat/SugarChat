using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Users
{
    public interface IUserDataProvider
    {
        Task<User> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetRangeByIdAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}