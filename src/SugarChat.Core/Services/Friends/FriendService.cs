using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Services.Friends
{
    public class FriendService : IFriendService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IFriendDataProvider _friendDataProvider;

        public FriendService(IMapper mapper, IUserDataProvider userDataProvider, IFriendDataProvider friendDataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _friendDataProvider = friendDataProvider;
        }

        public async Task<FriendAddedEvent> AddFriendAsync(AddFriendCommand command,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);

            user.CheckNotAddSelfAsFiend(command.UserId, command.FriendId);

            User friend = await GetUserAsync(command.FriendId, cancellation);
            friend.CheckExist(command.FriendId);

            Friend existFriend =
                await _friendDataProvider.GetByUsersIdAsync(command.UserId, command.FriendId, cancellation);
            existFriend.CheckNotExist(command.UserId, command.FriendId);

            Friend makeFriend = new Friend
            {
                Id = Guid.NewGuid().ToString(),
                UserId = command.UserId,
                FriendId = command.FriendId,
                BecomeFriendAt = DateTimeOffset.UtcNow
            };

            await _friendDataProvider.AddAsync(makeFriend, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = makeFriend.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<FriendRemovedEvent> RemoveFriendAsync(RemoveFriendCommand command,
            CancellationToken cancellation = default)
        {
            Friend friend = await _friendDataProvider.GetByUsersIdAsync(command.UserId, command.FriendId, cancellation);
            friend.CheckExist(command.UserId, command.FriendId);

            await _friendDataProvider.RemoveAsync(friend, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = friend.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<GetFriendsOfUserResponse> GetFriendsOfUserAsync(GetFriendsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(request.Id, cancellation);
            user.CheckExist(request.Id);

            IEnumerable<User> friends = await
                _userDataProvider.GetRangeByIdAsync(
                    (await _friendDataProvider.GetByUserIdAsync(request.Id, cancellation)).Select(o => o.FriendId),
                    cancellation);

            IEnumerable<UserDto> friendsDto = _mapper.Map<IEnumerable<UserDto>>(friends);
            return new()
            {
                Friends = friendsDto
            };
        }

        private Task<User> GetUserAsync(string id, CancellationToken cancellation = default)
        {
            return _userDataProvider.GetByIdAsync(id, cancellation);
        }
    }
}