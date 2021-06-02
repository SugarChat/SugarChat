using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserDataProvider : IGroupUserDataProvider
    {
        private const string UpdateGroupUserFailed = "GroupUser with Id {0} Update Failed.";
        private const string AddGroupUserFailed = "GroupUser with Id {0} Add Failed.";
        private const string RemoveGroupUserFailed = "GroupUser with Id {0} Remove Failed.";

        private readonly IRepository _repository;

        public GroupUserDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(GroupUser groupUser, CancellationToken cancellation = default)
        {
            int affectedLineNum = await _repository.AddAsync(groupUser, cancellation);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(AddGroupUserFailed, groupUser.Id));
            }
        }

        public async Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(o => o.UserId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUser>> GetByGroupIdAsync(string id,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(o => o.GroupId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId,
            CancellationToken cancellationToken = default)
        {
            return await _repository
                .SingleOrDefaultAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetMessageReadAsync(string userId, string groupId, DateTimeOffset messageSentTime,
            CancellationToken cancellationToken = default)
        {
            GroupUser groupUser =
                await _repository.SingleOrDefaultAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId,
                    cancellationToken);
            if (groupUser is null)
            {
                throw new ArgumentException();
            }

            groupUser.LastReadTime = messageSentTime;

            int affectedLineNum = await _repository.UpdateAsync(groupUser, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(UpdateGroupUserFailed, groupUser.Id));
            }
        }

        public async Task RemoveAsync(GroupUser groupUser, CancellationToken cancellation = default)
        {
            int affectedLineNum = await _repository.RemoveAsync(groupUser, cancellation);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(RemoveGroupUserFailed, groupUser.Id));
            }
        }
    }
}