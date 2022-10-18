using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUserCustomProperties
{
    public class GroupUserCustomPropertyDataProvider : IGroupUserCustomPropertyDataProvider
    {
        private readonly IRepository _repository;

        public GroupUserCustomPropertyDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddRangeAsync(IEnumerable<GroupUserCustomProperty> groupUserCustomProperty, CancellationToken cancellationToken = default)
        {
            await _repository.AddRangeAsync(groupUserCustomProperty, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUserCustomProperty>> GetPropertiesByGroupUserIds(IEnumerable<string> groupUserIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUserCustomProperty>(x => groupUserIds.Contains(x.GroupUserId), cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUserCustomProperty>> GetPropertiesByGroupUserId(string groupUserId, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUserCustomProperty>(x => x.GroupUserId == groupUserId, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<GroupUserCustomProperty> groupCustomProperties, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveRangeAsync(groupCustomProperties, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> FilterGroupUserByCustomProperties(IEnumerable<string> groupUserIds, Dictionary<string, List<string>> customProperties, CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            foreach (var dic in customProperties)
            {
                foreach (var value in dic.Value)
                {
                    var _value = value.Replace("\\", "\\\\");
                    var _key = dic.Key.Replace("\\", "\\\\");
                    sb.Append($" || (Key==\"{_key}\" && Value==\"{_value}\")");
                }
            }
            var where = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupUserCustomProperty>().Where(x => groupUserIds.Contains(x.GroupUserId)), sb.ToString().Substring(4));
            var groupUserCustomProperties = await _repository.ToListAsync(where, cancellationToken).ConfigureAwait(false);
            return groupUserCustomProperties.Select(x => x.GroupUserId).ToList();
        }
    }
}
