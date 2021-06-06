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

        public async Task<AddUserEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellation = default)
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

        public async Task<UpdateUserEvent> UpdateUserAsync(UpdateUserCommand command,
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

        public async Task<RemoveUserEvent> RemoveUserAsync(RemoveUserCommand command,
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
            User user = await _userDataProvider.GetByIdAsync(request.Id, cancellation);
            user.CheckExist(request.Id);
            return new()
            {
                User = _mapper.Map<UserDto>(await _userDataProvider.GetByIdAsync(request.Id, cancellation))
            };
        }

        public Task<GetCurrentUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request,
            CancellationToken cancellation = default)
        {
            return Task.FromResult(new GetCurrentUserResponse { User = new UserDto() });
        }


        public async Task<GetFriendsOfUserResponse> GetFriendsOfUserAsync(GetFriendsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(request.Id, cancellation);
            user.CheckExist(request.Id);

            PagedResult<Friend> friends = await _friendDataProvider.GetAllFriendsByUserIdAsync(request.Id,
                request.PageSettings, cancellation);
            IEnumerable<User> users = await
                _userDataProvider.GetRangeByIdAsync(friends.Result.Select(o => o.FriendId), cancellation);

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

        private Task<User> GetUserAsync(string id, CancellationToken cancellation = default)
        {
            return _userDataProvider.GetByIdAsync(id, cancellation);
        }
    }
}