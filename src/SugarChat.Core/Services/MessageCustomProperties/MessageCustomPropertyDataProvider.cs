using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.MessageCustomProperties
{
    public class MessageCustomPropertyDataProvider: IMessageCustomPropertyDataProvider
    {
        private readonly IRepository _repository;

        public MessageCustomPropertyDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddRangeAsync(IEnumerable<MessageCustomProperty> groupCustomProperty, CancellationToken cancellationToken = default)
        {
            await _repository.AddRangeAsync(groupCustomProperty, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<MessageCustomProperty>> GetPropertiesByMessageIds(IEnumerable<string> messageIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<MessageCustomProperty>(x => messageIds.Contains(x.MessageId), cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<MessageCustomProperty> messageCustomProperties, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveRangeAsync<MessageCustomProperty>(messageCustomProperties, cancellationToken).ConfigureAwait(false);
        }
    }
}
