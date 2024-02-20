using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Exceptions;
using SugarChat.Message.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroup2DataProvider : IDataProvider
    {
        Task<Group2> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<Group2> GetByIdAsync(Group group, CancellationToken cancellationToken = default);

        Task<List<Group2>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Task AddAsync(Group2 group, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<Group2> groups, CancellationToken cancellationToken = default);

        Task UpdateAsync(Group2 group, CancellationToken cancellationToken = default);

        int GetUnreadCount(string userId, int groupType);

        Task UpdateRangeAsync(IEnumerable<Group2> groups, CancellationToken cancellationToken = default);

        Task<PagedResult<Message2>> GetMessages(string groupId, PageSettings pageSettings, DateTimeOffset? fromDate, CancellationToken cancellationToken = default);

        Task<Group2> GetAsync(Expression<Func<Group2, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Group2>> GetListAsync(Expression<Func<Group2, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task RemoveAsync(Group2 group, CancellationToken cancellationToken = default);
    }

    public class Group2DataProvider : IGroup2DataProvider
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public Group2DataProvider(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Group2> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.SingleOrDefaultAsync<Group2>(x => x.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Group2> GetByIdAsync(Group group, CancellationToken cancellationToken = default)
        {
            var group2 = await _repository.SingleOrDefaultAsync<Group2>(x => x.Id == group.Id, cancellationToken)
                .ConfigureAwait(false);
            if (group2 != null)
            {
                return group2;
            }
            else
            {
                await _repository.AddAsync(_mapper.Map<Group2>(group), cancellationToken);
                return _mapper.Map<Group2>(group);
            }
        }

        public async Task<List<Group2>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<Group2>(x => ids.Contains(x.Id), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task AddAsync(Group2 group, CancellationToken cancellationToken = default)
        {
            await _repository.AddAsync(group, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddRangeAsync(IEnumerable<Group2> groups, CancellationToken cancellationToken = default)
        {
            await _repository.AddRangeAsync(groups, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Group2 group, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateAsync(group, cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<Group2> groups, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateRangeAsync(groups, cancellationToken);
        }

        public int GetUnreadCount(string userId, int groupType)
        {
            var unreadCount = _repository.Query<Group2>().Where(x => x.Type == groupType && x.GroupUsers.Any(y => y.UserId == userId))
                  .Select(x => x.GroupUsers.Where(y => y.UserId == userId)).ToList()
                  .SelectMany(x => x)
                  .Sum(x => x.UnreadCount);
            return unreadCount;
        }

        public async Task<PagedResult<Message2>> GetMessages(string groupId, PageSettings pageSettings, DateTimeOffset? fromDate, CancellationToken cancellationToken = default)
        {
            var group = await _repository.Query<Group2>().SingleOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return new PagedResult<Message2>();

            var messages = group.Messages;
            if (fromDate is not null)
            {
                messages = messages.Where(x => x.SentTime >= fromDate).ToList();
            }
            messages = messages.OrderByDescending(x => x.SentTime).ToList();
            var result = new PagedResult<Message2>();
            if (pageSettings is not null)
            {
                result = _repository.ToPagedListAsync(pageSettings, messages);
            }
            else
            {
                result.Result = messages;
                result.Total = messages.Count();
            }
            return result;
        }

        public async Task<Group2> GetAsync(Expression<Func<Group2, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return await _repository.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Group2>> GetListAsync(Expression<Func<Group2, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(Group2 group, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveAsync(group, cancellationToken).ConfigureAwait(false);
        }
    }
}
