using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Paging;

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

        Task<IEnumerable<Group>> GetByCustomProperties(IEnumerable<string> groupIds, Dictionary<string, string> customProperties, PageSettings pageSettings, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetGroupIdsByMessageKeywordAsync(IEnumerable<string> groupIds, Dictionary<string, string> searchParms, bool isExactSearch, CancellationToken cancellationToken = default, int? type = null);

        Task<int> GetCountAsync(Expression<Func<Group, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Group>> GetListAsync(PageSettings pageSettings, Expression<Func<Group, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task UpdateRangeAsync(IEnumerable<Group> groups, CancellationToken cancellationToken = default);
    }
}