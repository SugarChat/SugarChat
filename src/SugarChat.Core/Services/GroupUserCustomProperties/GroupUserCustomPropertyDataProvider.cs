using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
            if (customProperties == null || !customProperties.Any())
            {
                return groupUserIds;
            }
            var sb = new StringBuilder();
            foreach (var dic in customProperties)
            {
                foreach (var value in dic.Value)
                {
                    var value1 = value.Replace("\\", "\\\\");
                    var key1 = dic.Key.Replace("\\", "\\\\");
                    if (value1.Contains(","))
                    {
                        var values = value1.Split(',');
                        foreach (var value2 in values)
                        {
                            sb.Append($" || (Key==\"{key1}\" && Value==\"{value2}\")");
                        }
                    }
                    else
                        sb.Append($" || (Key==\"{key1}\" && Value==\"{value1}\")");
                }
            }
            var query = _repository.Query<GroupUserCustomProperty>();
            if (groupUserIds != null && groupUserIds.Any())
            {
                query = query.Where(x => groupUserIds.Contains(x.GroupUserId));
            }
            var groupUserCustomProperties = await _repository.ToListAsync(query.Where(sb.ToString().Substring(4)), cancellationToken).ConfigureAwait(false);
            return groupUserCustomProperties.Select(x => x.GroupUserId).ToList();
        }
    }
}
