using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
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

        public async Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<GroupUser>(o => o.UserId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId,
            CancellationToken cancellationToken)
        {
            return await _repository
                .SingleOrDefaultAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(GroupUser groupUser, CancellationToken cancellation)
        {
            await _repository.UpdateAsync(groupUser, cancellation);
        }

        public async Task<IEnumerable<GroupUserDto>> GetGroupMembersByIdAsync(string id, CancellationToken cancellationToken)
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

        public async Task RemoveAsync(GroupUser groupUser, CancellationToken cancellation = default)
        {
            await _repository.RemoveAsync(groupUser, cancellation);
        }
    }
}