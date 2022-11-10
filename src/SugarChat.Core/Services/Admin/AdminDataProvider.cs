using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var groupUserIds = _repository.Query<GroupUserCustomProperty>().GroupBy(x => new { x.GroupUserId, x.Key, x.Value }).Where(x => x.Count() > 1).Select(x => x.Key.GroupUserId).ToList();
                groupUserIds = groupUserIds.Distinct().ToList();
                var groupUserCustomProperties = _repository.Query<GroupUserCustomProperty>().Where(x => groupUserIds.Contains(x.GroupUserId)).ToList();

                var needDeleteGroupUserCustomProperties = new List<GroupUserCustomProperty>();
                foreach (var groupUserId in groupUserIds)
                {
                    var _groupUserCustomProperties = groupUserCustomProperties.Where(x => x.GroupUserId == groupUserId).ToList();
                    var groupBys = _groupUserCustomProperties.GroupBy(x => new { x.Key, x.Value }).ToList();
                    foreach (var groupBy in groupBys)
                    {
                        if (groupBy.Count() > 1)
                        {
                            needDeleteGroupUserCustomProperties.AddRange(groupBy.OrderBy(x => x.CreatedDate).Skip(1).ToList());
                        }
                    }
                }
                await _repository.RemoveRangeAsync(needDeleteGroupUserCustomProperties);
            }
            {
                var messageIds = _repository.Query<MessageCustomProperty>().GroupBy(x => new { x.MessageId, x.Key, x.Value }).Where(x => x.Count() > 1).Select(x => x.Key.MessageId).ToList();
                messageIds = messageIds.Distinct().ToList();
                var messageCustomProperties = _repository.Query<MessageCustomProperty>().Where(x => messageIds.Contains(x.MessageId)).ToList();

                var needDeleteMessageCustomProperties = new List<MessageCustomProperty>();
                foreach (var messageId in messageIds)
                {
                    var _messageCustomProperties = messageCustomProperties.Where(x => x.MessageId == messageId).ToList();
                    var groupBys = _messageCustomProperties.GroupBy(x => new { x.Key, x.Value }).ToList();
                    foreach (var groupBy in groupBys)
                    {
                        if (groupBy.Count() > 1)
                        {
                            needDeleteMessageCustomProperties.AddRange(groupBy.OrderBy(x => x.CreatedDate).Skip(1).ToList());
                        }
                    }
                }
                await _repository.RemoveRangeAsync(needDeleteMessageCustomProperties);
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
