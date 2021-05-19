using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroupDataProvider
    {
        Task<Group> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<IEnumerable<Group>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken);
        Task AddAsync(Group group, CancellationToken cancellation);
        Task UpdateAsync(Group group, CancellationToken cancellation);
        Task RemoveAsync(Group group, CancellationToken cancellation);
    }
}