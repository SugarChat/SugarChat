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
                            needDeleteGroupCustomProperties.AddRange(groupBy.OrderBy(x => x.CreatedBy).Skip(1).ToList());
                        }
                    }
                }
                //await _repository.RemoveRangeAsync(needDeleteGroupCustomProperties);

                //var ccc = groupCustomProperties.Count(x => x.CreatedBy == null);
                //var aaa = groupCustomProperties.Where(x => !needDeleteGroupCustomProperties.Select(y => y.Id).Contains(x.Id) && x.CreatedBy != null).ToList();
                //var bbb = needDeleteGroupCustomProperties.Any(x => x.CreatedBy == null);
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
                            needDeleteGroupUserCustomProperties.AddRange(groupBy.OrderBy(x => x.CreatedBy).Skip(1).ToList());
                        }
                    }
                }
            }
        }
    }
}
