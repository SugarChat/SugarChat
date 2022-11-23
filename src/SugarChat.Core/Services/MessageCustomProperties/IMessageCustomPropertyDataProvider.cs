using SugarChat.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.MessageCustomProperties
{
    public interface IMessageCustomPropertyDataProvider : IDataProvider
    {
        Task AddRangeAsync(IEnumerable<MessageCustomProperty> groupCustomProperty, CancellationToken cancellationToken = default);

        Task<IEnumerable<MessageCustomProperty>> GetPropertiesByMessageIds(IEnumerable<string> messageIds, CancellationToken cancellationToken = default);

        Task RemoveRangeAsync(IEnumerable<MessageCustomProperty> messageCustomProperties, CancellationToken cancellationToken = default);
    }
}
