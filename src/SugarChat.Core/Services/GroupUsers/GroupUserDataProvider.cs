using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserDataProvider : IGroupUserDataProvider
    {
        private readonly IRepository _repository;

        public GroupUserDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(string userId, string groupId, CancellationToken cancellation)
        {
            GroupUser groupUser = new()
            {
                //TODO should talk about this later
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                GroupId = groupId
            };
            await _repository.AddAsync(groupUser, cancellation);
        }

        public async Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<GroupUser>(o => o.UserId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUser>> GetByGroupIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<GroupUser>(o => o.GroupId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId,
            CancellationToken cancellationToken)
        {
            return await _repository
                .SingleOrDefaultAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetMessageReadByUserAsync(string userId, string groupId, DateTimeOffset messageSentTime,
            CancellationToken cancellationToken)
        {
            GroupUser groupUser =
                await _repository.SingleOrDefaultAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId,
                    cancellationToken);
            groupUser.LastReadTime = messageSentTime;
            await _repository.UpdateAsync(groupUser, cancellationToken);
        }

        public async Task RemoveAsync(GroupUser groupUser, CancellationToken cancellation)
        {
            await _repository.RemoveAsync(groupUser, cancellation);
        }
    }
}