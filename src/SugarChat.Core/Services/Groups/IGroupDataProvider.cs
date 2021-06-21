using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Shared.Paging;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroupDataProvider : IDataProvider
    {
        Task<Group> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<PagedResult<Group>> GetByIdsAsync(IEnumerable<string> ids, PageSettings pageSettings,
            CancellationToken cancellationToken = default);

        Task AddAsync(Group group, CancellationToken cancellationToken = default);
        Task UpdateAsync(Group group, CancellationToken cancellationToken = default);
        Task RemoveAsync(Group group, CancellationToken cancellationToken = default);

        IEnumerable<Group> GetByCustomPropertys(Dictionary<string, string> customPropertys, IEnumerable<string> groupIds = null);
    }
}