using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Dtos.GroupUsers;
using System.Linq;
using System.Linq.Expressions;
using SugarChat.Message.Paging;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserDataProvider : IGroupUserDataProvider
    {
        private readonly IRepository _repository;

        public GroupUserDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(GroupUser groupUser, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.AddAsync(groupUser, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.AddGroupUserFailed.WithParams(groupUser.Id));
            }
        }

        public async Task<IEnumerable<GroupUser>> GetByUserIdAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default, int? type = null)
        {
            if (groupIds == null)
                groupIds = new List<string>();
            var groupUsers = (from a in _repository.Query<GroupUser>()
                              join b in _repository.Query<Group>() on a.GroupId equals b.Id
                              where a.UserId == userId && (b.Type == type || (type == 0 && b.Type == null)) && (!groupIds.Any() || groupIds.Contains(b.Id))
                              select new
                              {
                                  a.Id,
                                  a.UserId,
                                  a.GroupId,
                                  a.Role,
                                  a.MessageRemindType
                              }).ToList();
            var result = new List<GroupUser>();
            foreach (var groupUser in groupUsers)
            {
                result.Add(new GroupUser
                {
                    Id = groupUser.Id,
                    UserId = groupUser.UserId,
                    GroupId = groupUser.GroupId,
                    Role = groupUser.Role,
                    MessageRemindType = groupUser.MessageRemindType
                });
            }
            return result;
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

        public async Task RemoveAsync(GroupUser groupUser, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.RemoveAsync(groupUser, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.RemoveGroupUserFailed.WithParams(groupUser.Id));
            }
        }

        public async Task UpdateAsync(GroupUser groupUser, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.UpdateAsync(groupUser, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.UpdateGroupUserFailed.WithParams(groupUser.Id));
            }
        }

        public async Task<IEnumerable<GroupUser>> GetMembersByGroupIdAsync(string id,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(x => x.GroupId == id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> GetGroupMemberCountByGroupIdAsync(string groupId, CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync<GroupUser>(x => x.GroupId == groupId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task AddRangeAsync(IEnumerable<GroupUser> groupUsers,
            CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.AddRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != groupUsers.Count())
            {
                throw new BusinessWarningException(Prompt.AddGroupUsersFailed.WithParams(groupUsers.Count().ToString(),
                    affectedLineNum.ToString()));
            }
        }

        public async Task<IEnumerable<GroupUser>> GetByGroupIdAndUsersIdAsync(string groupId,
            IEnumerable<string> userIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(o => o.GroupId == groupId && userIds.Contains(o.UserId),
                cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<GroupUser> groupUsers,
            CancellationToken cancellationToken = default)
        {
            int affectedLineNum =
                await _repository.UpdateRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != groupUsers.Count())
            {
                throw new BusinessWarningException(Prompt.UpdateGroupUsersFailed.WithParams(
                    groupUsers.Count().ToString(),
                    affectedLineNum.ToString()));
            }
        }

        public async Task RemoveRangeAsync(IEnumerable<GroupUser> groupUsers,
            CancellationToken cancellationToken = default)
        {
            int affectedLineNum =
                await _repository.RemoveRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != groupUsers.Count())
            {
                throw new BusinessWarningException(Prompt.RemoveGroupUsersFailed.WithParams(
                    groupUsers.Count().ToString(),
                    affectedLineNum.ToString()));
            }
        }

        public async Task<IEnumerable<GroupUser>> GetByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId), cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUser>> GetGroupMemberCountByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId), cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUser>> GetListByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(x => ids.Contains(x.Id), cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> GetCountAsync(Expression<Func<GroupUser, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUser>> GetListAsync(PageSettings pageSettings, Expression<Func<GroupUser, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return (await _repository.ToPagedListAsync(pageSettings, predicate, cancellationToken).ConfigureAwait(false)).Result;
        }
    }
}
