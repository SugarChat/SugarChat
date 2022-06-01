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

        Task<IEnumerable<GroupCustomProperty>> GetPropertyByGroupId(string groupId, CancellationToken cancellationToken = default);
    }
}
