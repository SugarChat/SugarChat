using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Shared.Dtos.GroupUsers;
using System.Linq;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserDataProvider : IGroupUserDataProvider
    {
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
                throw new BusinessWarningException(Prompt.AddGroupUserFailed.WithParams(groupUser.Id));
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
                throw new BusinessWarningException(Prompt.UpdateGroupUserFailed.WithParams(groupUser.Id));
            }
        }

        public async Task RemoveAsync(GroupUser groupUser, CancellationToken cancellation = default)
        {
            int affectedLineNum = await _repository.RemoveAsync(groupUser, cancellation);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.RemoveGroupUserFailed.WithParams(groupUser.Id));
            }
        }

        public async Task UpdateAsync(GroupUser groupUser, CancellationToken cancellation)
        {
            await _repository.UpdateAsync(groupUser, cancellation);
        }

        public async Task<IEnumerable<GroupUserDto>> GetMembersByGroupIdAsync(string id, CancellationToken cancellationToken)
        {
            return await Task.Run<IEnumerable<GroupUserDto>>(() =>
              {
                  return (from a in _repository.Query<GroupUser>()
                          join b in _repository.Query<User>() on a.UserId equals b.Id
                          where a.GroupId == id
                          select new GroupUserDto
                          {
                              UserId = a.UserId,
                              DisplayName = b.DisplayName,
                              AvatarUrl = b.AvatarUrl,
                              CustomProperties = b.CustomProperties,
                              JoinTime = a.CreatedDate
                          }).ToList();
              });
        }

       public async Task<int> GetGroupMemberCountAsync(string groupId, CancellationToken cancellationToken)
        {
            return await _repository.CountAsync<GroupUser>(x => x.GroupId == groupId, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddRangeAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellation)
        {
            await _repository.AddRangeAsync(groupUsers, cancellation).ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellationToken)
        {
            await _repository.RemoveRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
        }
    }
}