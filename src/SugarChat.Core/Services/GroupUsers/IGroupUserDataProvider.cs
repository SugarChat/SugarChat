using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Shared.Dtos.GroupUsers;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserDataProvider : IDataProvider
    {
        Task AddAsync(GroupUser groupUser, CancellationToken cancellation = default);
        Task RemoveAsync(GroupUser groupUser, CancellationToken cancellation = default);
        Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<GroupUser>> GetByGroupIdAsync(string id, CancellationToken cancellationToken = default);

        Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId,
            CancellationToken cancellationToken = default);

        Task SetMessageReadAsync(string userId, string groupId, DateTimeOffset messageSentTime,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(GroupUser groupUser, CancellationToken cancellation = default);

        Task<IEnumerable<GroupUserDto>> GetMembersByGroupIdAsync(string id,
            CancellationToken cancellationToken = default);

        Task<int> GetGroupMemberCountAsync(string groupId, CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellation = default);
    }
}