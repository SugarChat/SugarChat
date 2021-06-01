using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Shared.Dtos.GroupUsers;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserDataProvider: IDataProvider
    {
        Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken);
        Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId, CancellationToken cancellationToken);
        Task UpdateAsync(GroupUser groupUser, CancellationToken cancellation);
        Task<IEnumerable<GroupUserDto>> GetGroupMembersByIdAsync(string id, CancellationToken cancellationToken);
        Task RemoveAsync(GroupUser groupUser, CancellationToken cancellation = default);
    }
}