using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserDataProvider : IDataProvider
    {
        Task AddAsync(string userId, string groupId, CancellationToken cancellation);
        Task RemoveAsync(GroupUser groupUser, CancellationToken cancellation);
        Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken);
        Task<IEnumerable<GroupUser>> GetByGroupIdAsync(string id, CancellationToken cancellationToken);
        Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId, CancellationToken cancellationToken);
        Task SetMessageReadByUserAsync(string userId, string groupId, DateTimeOffset messageSentTime, CancellationToken cancellationToken);
    }
}