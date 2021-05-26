using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserDataProvider: IDataProvider
    {
        Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken);
        Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId, CancellationToken cancellationToken);
    }
}