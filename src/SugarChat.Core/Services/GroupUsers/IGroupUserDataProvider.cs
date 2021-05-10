using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserDataProvider
    {
        Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken);
    }
}