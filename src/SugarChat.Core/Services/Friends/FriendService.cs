using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Friends;
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

        public async Task<AddFriendEvent> AddFriendAsync(AddFriendCommand command,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);

            user.CheckNotAddSelfAsFiend(command.UserId, command.FriendId);

            User friend = await GetUserAsync(command.FriendId, cancellation);
            friend.CheckExist(command.FriendId);

            Friend existFriend =
                await _friendDataProvider.GetByBothIdsAsync(command.UserId, command.FriendId, cancellation);
            existFriend.CheckNotExist(command.UserId, command.FriendId);

            Friend makeFriend = _mapper.Map<Friend>(command);
            await _friendDataProvider.AddAsync(makeFriend, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = makeFriend.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<RemoveFriendEvent> RemoveFriendAsync(RemoveFriendCommand command,
            CancellationToken cancellation = default)
        {
            Friend friend = await _friendDataProvider.GetByBothIdsAsync(command.UserId, command.FriendId, cancellation);
            friend.CheckExist(command.UserId, command.FriendId);

            await _friendDataProvider.RemoveAsync(friend, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = friend.Id,
                Status = EventStatus.Success
            };
        }

        private Task<User> GetUserAsync(string id, CancellationToken cancellation = default)
        {
            return _userDataProvider.GetByIdAsync(id, cancellation);
        }
    }
}