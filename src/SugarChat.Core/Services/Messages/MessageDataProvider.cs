using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Messages
{
    public class MessageDataProvider : IMessageDataProvider
    {
        private readonly IRepository _repository;

        public MessageDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(Domain.Message message, CancellationToken cancellation)
        {
            await _repository.AddAsync(message, cancellation).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Domain.Message message, CancellationToken cancellation)
        {
            await _repository.UpdateAsync(message, cancellation).ConfigureAwait(false);
        }

        public async Task RemoveAsync(Domain.Message message, CancellationToken cancellation)
        {
            await _repository.RemoveAsync(message, cancellation).ConfigureAwait(false);
        }

        public async Task<Domain.Message> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.FirstOrDefaultAsync<Domain.Message>(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Domain.Message>> GetUnreadToUserFromFriendAsync(string userId, string friendId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetAllHistoryToUserFromFriendAsync(string userId, string friendId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetAllHistoryToUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetUnreadToUserFromGroupAsync(string userId, string groupId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetAllToUserFromGroupAsync(string userId, string groupId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Domain.Message>> GetByGroupIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<Domain.Message>(x => x.GroupId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<Domain.Message> messages, CancellationToken cancellationToken)
        {
            await _repository.RemoveRangeAsync(messages, cancellationToken).ConfigureAwait(false);
        }
    }
}