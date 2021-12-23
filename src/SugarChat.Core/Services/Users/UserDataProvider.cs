using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Users
{
    public class UserDataProvider : IUserDataProvider
    {
        private readonly IRepository _repository;

        public UserDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.SingleOrDefaultAsync<User>(x => x.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetRangeByIdAsync(IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<User>(o => ids.Contains(o.Id), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.AddAsync(user, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.AddUserFailed.WithParams(user.Id));
            }
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.UpdateUserFailed.WithParams(user.Id));
            }
        }

        public async Task RemoveAsync(User user, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.RemoveAsync(user, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.RemoveUserFailed.WithParams(user.Id));
            }
        }

        public async Task AddRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken = default)
        {
            await _repository.AddRangeAsync(users, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveRangeAsync(users, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetListAsync(Expression<Func<User, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
    }
}