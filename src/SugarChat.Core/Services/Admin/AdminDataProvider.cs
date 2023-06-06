using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System.Collections.Generic;
using System.Linq;

namespace SugarChat.Core.Services.Admin
{
    public class AdminDataProvider : IAdminDataProvider
    {
        private readonly IRepository _repository;

        public AdminDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 修复数据，临时使用
        /// </summary>
        public async void RepairData()
        {
            {
                var groupIds = _repository.Query<GroupCustomProperty>().GroupBy(x => new { x.GroupId, x.Key, x.Value }).Where(x => x.Count() > 1).Select(x => x.Key.GroupId).ToList();
                groupIds = groupIds.Distinct().ToList();
                var groupCustomProperties = _repository.Query<GroupCustomProperty>().Where(x => groupIds.Contains(x.GroupId)).ToList();

                var needDeleteGroupCustomProperties = new List<GroupCustomProperty>();
                foreach (var groupId in groupIds)
                {
                    var _groupCustomProperties = groupCustomProperties.Where(x => x.GroupId == groupId).ToList();
                    var groupBys = _groupCustomProperties.GroupBy(x => new { x.Key, x.Value }).ToList();
                    foreach (var groupBy in groupBys)
                    {
                        if (groupBy.Count() > 1)
                        {
                            needDeleteGroupCustomProperties.AddRange(groupBy.OrderBy(x => x.CreatedDate).Skip(1).ToList());
                        }
                    }
                }
                await _repository.RemoveRangeAsync(needDeleteGroupCustomProperties);
            }
            {
                var groupUserGroups = _repository.Query<GroupUser>().GroupBy(x => new { x.GroupId, x.UserId }).Where(x => x.Count() > 1).ToList();
                var groupIds = groupUserGroups.Select(x => x.Key.GroupId).ToList();
                var userIds = groupUserGroups.Select(x => x.Key.UserId).ToList();
                var groupUsers = _repository.Query<GroupUser>().Where(x => groupIds.Contains(x.GroupId) && userIds.Contains(x.UserId)).ToList();

                var needDeleteGroupUsers = new List<GroupUser>();
                foreach (var groupUserGroup in groupUserGroups)
                {
                    var _groupUser = groupUsers.Where(x => x.GroupId == groupUserGroup.Key.GroupId && x.UserId == groupUserGroup.Key.UserId).OrderBy(x => x.CreatedDate).Skip(1).ToList();
                    needDeleteGroupUsers.AddRange(_groupUser);
                }
                await _repository.RemoveRangeAsync(needDeleteGroupUsers);
            }
        }
    }
}
