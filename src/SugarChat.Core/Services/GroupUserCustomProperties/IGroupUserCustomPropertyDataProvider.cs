using SugarChat.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUserCustomProperties
{
    public interface IGroupUserCustomPropertyDataProvider : IDataProvider
    {
        Task AddRangeAsync(IEnumerable<GroupUserCustomProperty> groupUserCustomProperty, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupUserCustomProperty>> GetPropertiesByGroupUserIds(IEnumerable<string> groupUserIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<GroupUserCustomProperty>> GetPropertiesByGroupUserId(string groupUserId, CancellationToken cancellationToken = default);

        Task RemoveRangeAsync(IEnumerable<GroupUserCustomProperty> groupCustomProperties, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> FilterGroupUserByCustomProperties(IEnumerable<string> groupUserIds, Dictionary<string, List<string>> customProperties, CancellationToken cancellationToken = default);
    }
}
