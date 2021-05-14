using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Friends;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IFriendDataProvider _friendDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;

        //TODO Error const would be moved to specific class or json file


        public UserService(IMapper mapper, IUserDataProvider userDataProvider,
            IFriendDataProvider friendDataProvider, IGroupUserDataProvider groupUserDataProvider,
            IGroupDataProvider groupDataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _friendDataProvider = friendDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _groupDataProvider = groupDataProvider;
        }


        public async Task<UserAddedEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellation);
            user.CheckNotExist();

            user = _mapper.Map<User>(command);
            await _userDataProvider.AddAsync(user, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = user.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<UserUpdatedEvent> UpdateUserAsync(UpdateUserCommand command,
            CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellation);
            user.CheckExist(command.Id);

            user = _mapper.Map<User>(command);
            await _userDataProvider.UpdateAsync(user, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = user.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<UserDeletedEvent> DeleteUserAsync(DeleteUserCommand command,
            CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellation);
            user.CheckExist(command.Id);

            await _userDataProvider.RemoveAsync(user, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = command.Id,
                Status = EventStatus.Success,
            };
        }

        public async Task<GetUserResponse> GetUserAsync(GetUserRequest request,
            CancellationToken cancellation = default)
        {
            return new()
            {
                User = _mapper.Map<UserDto>(await _userDataProvider.GetByIdAsync(request.Id, cancellation))
            };
        }

        public Task<GetUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request,
            CancellationToken cancellation = default)
        {
            return Task.FromResult(new GetUserResponse {User = new UserDto()});
        }

        public Task<GetMembersOfGroupResponse> GetMembersOfGroupAsync(GetMembersOfGroupRequest request, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }
    }
}