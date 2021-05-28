using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            await _repository.AddAsync(message, cancellation);
        }

        public async Task UpdateAsync(Domain.Message message, CancellationToken cancellation)
        {
            await _repository.UpdateAsync(message, cancellation);
        }

        public async Task RemoveAsync(Domain.Message message, CancellationToken cancellation)
        {
            await _repository.RemoveAsync(message, cancellation);
        }

        public async Task<Domain.Message> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.FirstOrDefaultAsync<Domain.Message>(x => x.Id == id, cancellationToken);
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
    }
}