using SugarChat.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupCustomProperties
{
    public interface IGroupCustomPropertyDataProvider : IDataProvider
    {
        Task AddRangeAsync(IEnumerable<GroupCustomProperty> groupCustomProperty, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupCustomProperty>> GetPropertiesByGroupIds(IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupCustomProperty>> GetPropertiesByGroupId(string groupId, CancellationToken cancellationToken = default);

        Task RemoveRangeAsync(IEnumerable<GroupCustomProperty> groupCustomProperties, CancellationToken cancellationToken = default);
    }
}
