using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Events.Groups;

namespace SugarChat.Core.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repository;
        private readonly IGroupDataProvider _groupDataProvider;

        public GroupService(IMapper mapper, IRepository repository, IGroupDataProvider groupDataProvider)
        {
            _mapper = mapper;
            _repository = repository;
            _groupDataProvider = groupDataProvider;
        }

        public async Task<GroupAddedEvent> AddGroupAsync(AddGroupCommand command, CancellationToken cancellation)
        {
            var group = _mapper.Map<Group>(command);

            await _repository.AddAsync(group, cancellation).ConfigureAwait(false);

            return new GroupAddedEvent
            {
                Id = group.Id
            };
        }
    }
}