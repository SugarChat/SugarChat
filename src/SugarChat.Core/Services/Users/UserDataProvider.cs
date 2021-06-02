using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Users
{
    public class UserDataProvider : IUserDataProvider
    {
        private const string UpdateUserFailed = "User with Id {0} Update Failed.";
        private const string AddUserFailed = "User with Id {0} Add Failed.";
        private const string RemoveUserFailed = "User with Id {0} Remove Failed.";

        private readonly IRepository _repository;

        public UserDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken)
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

        public async Task AddAsync(User user, CancellationToken cancellation = default)
        {
            int affectedLineNum = await _repository.AddAsync(user, cancellation).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(AddUserFailed, user.Id));
            }
        }

        public async Task UpdateAsync(User user, CancellationToken cancellation = default)
        {
            int affectedLineNum = await _repository.UpdateAsync(user, cancellation).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(UpdateUserFailed, user.Id));
            }
        }

        public async Task RemoveAsync(User user, CancellationToken cancellation = default)
        {
            int affectedLineNum = await _repository.RemoveAsync(user, cancellation).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(RemoveUserFailed, user.Id));
            }
        }
    }
}