using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Dtos;
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

        Task<IEnumerable<Group>> GetByCustomPropertiesAsync(IEnumerable<string> groupIds, Dictionary<string, string> customProperties, PageSettings pageSettings, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetGroupIdByIncludeCustomPropertiesAsync(IEnumerable<string> filterGroupIds, SearchGroupByGroupCustomPropertiesDto includeGroupByGroupCustomProperties, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetGroupIdByExcludeCustomPropertiesAsync(IEnumerable<string> filterGroupIds, SearchGroupByGroupCustomPropertiesDto excludeGroupByGroupCustomProperties, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetGroupIdsByMessageKeywordAsync(IEnumerable<string> filterGroupIds, Dictionary<string, string> searchParms, bool isExactSearch, int groupType, CancellationToken cancellationToken = default);

        Task<int> GetCountAsync(Expression<Func<Group, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Group>> GetListAsync(PageSettings pageSettings, Expression<Func<Group, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task UpdateRangeAsync(IEnumerable<Group> groups, CancellationToken cancellationToken = default);

        IEnumerable<string> GetGroupIds(Expression<Func<Group, bool>> predicate = null);

        Task<(IEnumerable<string>, int)> GetGroupIdsAsync(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            PageSettings pageSettings,
            Dictionary<string, string> searchParms, bool isExactSearch,
            SearchGroupByGroupCustomPropertiesDto includeGroupByGroupCustomProperties,
            SearchGroupByGroupCustomPropertiesDto excludeGroupByGroupCustomProperties,
            CancellationToken cancellationToken = default);
    }
}