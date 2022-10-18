using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Dtos.GroupUsers;
using SugarChat.Message.Paging;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserDataProvider : IDataProvider
    {
        Task AddAsync(GroupUser groupUser, CancellationToken cancellationToken = default);
        Task RemoveAsync(GroupUser groupUser, CancellationToken cancellationToken = default);
        Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken = default, int? type = null);
        Task<IEnumerable<GroupUser>> GetByGroupIdAsync(string id, CancellationToken cancellationToken = default);

        Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId,
            CancellationToken cancellationToken = default);

        Task SetMessageReadAsync(string userId, string groupId, DateTimeOffset messageSentTime,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(GroupUser groupUser, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupUser>> GetMembersByGroupIdAsync(string id,
            CancellationToken cancellationToken = default);

        Task<int> GetGroupMemberCountByGroupIdAsync(string groupId, CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupUser>> GetByGroupIdAndUsersIdAsync(string groupId, IEnumerable<string> userIds,
            CancellationToken cancellationToken = default);

        Task UpdateRangeAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupUser>> GetByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupUser>> GetGroupMemberCountByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupUser>> GetListByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync(Expression<Func<GroupUser, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<GroupUser>> GetListAsync(PageSettings pageSettings, Expression<Func<GroupUser, bool>> predicate = null, CancellationToken cancellationToken = default);
    }
}