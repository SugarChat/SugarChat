using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserService : IGroupUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public GroupUserService(IMapper mapper, IGroupDataProvider groupDataProvider, IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
        }


        public Task<AddUserToGroupEvent> AddUserToGroupAsync(AddUserToGroupCommand command, CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }

        public Task<RemoveUserFromGroupEvent> RemoveUserFromGroupAsync(RemoveUserFromGroupCommand command, CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetGroupMembersResponse> GetGroupMembersAsync(GetGroupMembersRequest request, CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }
    }
}