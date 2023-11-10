using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Groups
{
    public interface IGroup2DataProvider : IDataProvider
    {
        Task<Group2> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<Group2> GetByIdAsync(Group group, CancellationToken cancellationToken = default);

        Task AddAsync(Group2 group, CancellationToken cancellationToken = default);

        Task UpdateAsync(Group2 group, CancellationToken cancellationToken = default);
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

        public async Task AddAsync(Group2 group, CancellationToken cancellationToken = default)
        {
            await _repository.AddAsync(group, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Group2 group, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateAsync(group, cancellationToken);
        }
    }
}
