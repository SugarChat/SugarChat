using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Friends;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Shared.Dtos;
using SugarChat.Shared.Paging;

namespace SugarChat.Core.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IFriendDataProvider _friendDataProvider;

        public UserService(IMapper mapper, IUserDataProvider userDataProvider, IFriendDataProvider friendDataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _friendDataProvider = friendDataProvider;
        }

        public async Task<UserAddedEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellationToken = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
            user.CheckNotExist();

            user = _mapper.Map<User>(command);
            await _userDataProvider.AddAsync(user, cancellationToken).ConfigureAwait(false);

            return new()
            {
                Id = user.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<UserUpdatedEvent> UpdateUserAsync(UpdateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.Id);

            user = _mapper.Map<User>(command);
            await _userDataProvider.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<UserUpdatedEvent>(command);
        }

        public async Task<UserRemovedEvent> RemoveUserAsync(RemoveUserCommand command,
            CancellationToken cancellationToken = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.Id);

            await _userDataProvider.RemoveAsync(user, cancellationToken).ConfigureAwait(false);

            return new()
            {
                Id = command.Id,
                Status = EventStatus.Success,
            };
        }

        public async Task<GetUserResponse> GetUserAsync(GetUserRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await _userDataProvider.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
            user.CheckExist(request.Id);
            return new()
            {
                User = _mapper.Map<UserDto>(await _userDataProvider.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false))
            };
        }

        public Task<GetCurrentUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new GetCurrentUserResponse {User = new UserDto()});
        }


        public async Task<GetFriendsOfUserResponse> GetFriendsOfUserAsync(GetFriendsOfUserRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.Id, cancellationToken).ConfigureAwait(false);
            user.CheckExist(request.Id);

            PagedResult<Friend> friends = await _friendDataProvider.GetAllFriendsByUserIdAsync(request.Id,
                request.PageSettings, cancellationToken).ConfigureAwait(false);
            IEnumerable<User> users = await
                _userDataProvider.GetRangeByIdAsync(friends.Result.Select(o => o.FriendId), cancellationToken).ConfigureAwait(false);

            IEnumerable<UserDto> friendsDto = _mapper.Map<IEnumerable<UserDto>>(users);
            return new()
            {
                Friends = new PagedResult<UserDto>
                {
                    Result = friendsDto,
                    Total = friends.Total
                }
            };
        }

        private async Task<User> GetUserAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _userDataProvider.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        }
    }
}