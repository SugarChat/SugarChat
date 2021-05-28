using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Shared.Paging;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroupDataProvider: IDataProvider
    {
        Task<Group> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<PagedResult<Group>> GetByIdsAsync(IEnumerable<string> ids, PageSettings pageSettings, CancellationToken cancellationToken);
        Task AddAsync(Group group, CancellationToken cancellation);
        Task UpdateAsync(Group group, CancellationToken cancellation);
        Task RemoveAsync(Group group, CancellationToken cancellation);
    }
}