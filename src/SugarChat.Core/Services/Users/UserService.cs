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
        private const string UserExists = "User with Id {0} is already existed.";
        private const string UserNoExists = "User with Id {0} Dose not exist.";
        private const string FriendAlreadyMade = "User with Id {0} has already made friend with Id {1}.";
        private const string AddSelfAsFiend = "User with Id {0} Should not add self as friend.";
        private const string NotFriend = "User with Id {0} is not friend with Id {1} yet.";

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
            CheckUserNoExists(user);

            user = _mapper.Map<User>(command);
            await _userDataProvider.AddAsync(user, cancellation).ConfigureAwait(false);

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
            CheckUserExists(user, command.Id);

            await _userDataProvider.RemoveAsync(user, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = command.Id,
                Status = EventStatus.Success,
            };
        }

        public async Task<FriendAddedEvent> AddFriendAsync(AddFriendCommand command,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(command.UserId, cancellation);
            CheckUserExists(user, command.UserId);

            CheckNotAddSelfAsFiend(command.UserId, command.FriendId);

            User friend = await GetUserAsync(command.FriendId, cancellation);
            CheckUserExists(friend, command.FriendId);

            Friend existFriend =
                await _friendDataProvider.GetByUsersIdAsync(command.UserId, command.FriendId, cancellation);
            CheckNotFriend(existFriend, command.UserId, command.FriendId);

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
            CheckFriend(friend, command.UserId, command.FriendId);

            await _friendDataProvider.RemoveAsync(friend, cancellation).ConfigureAwait(false);

            return new()
            {
                Id = friend.Id,
                Status = EventStatus.Success
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

        public async Task<GetFriendsOfUserResponse> GetFriendsOfUserAsync(GetFriendsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            //TODO Should I check if the userId is invalid and return a failed Response with Information of BusinessException?
            User user = await GetUserAsync(request.Id, cancellation);
            CheckUserExists(user, request.Id);

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

        public async Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(request.Id, cancellation);
            CheckUserExists(user, request.Id);

            IEnumerable<GroupUser> groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.Id, cancellation);
            IEnumerable<Group> groups =
                await _groupDataProvider.GetByIdsAsync(groupUsers.Select(o => o.GroupId), cancellation);
            IEnumerable<GroupDto> groupsDto = _mapper.Map<IEnumerable<GroupDto>>(groups);
            return new()
            {
                Friends = groupsDto
            };
        }

        private Task<User> GetUserAsync(string id, CancellationToken cancellation = default)
        {
            return _userDataProvider.GetByIdAsync(id, cancellation);
        }

        private void CheckUserNoExists(User user)
        {
            if (user is not null)
            {
                throw new BusinessWarningException(string.Format(UserExists, user.Id));
            }
        }

        private void CheckUserExists(User user, string id)
        {
            if (user is null)
            {
                throw new BusinessWarningException(string.Format(UserNoExists, id));
            }
        }

        private void CheckNotFriend(Friend friend, string userId, string friendId)
        {
            if (friend is not null)
            {
                throw new BusinessWarningException(string.Format(FriendAlreadyMade, userId, friendId));
            }
        }

        private void CheckFriend(Friend friend, string userId, string friendId)
        {
            if (friend is null)
            {
                throw new BusinessWarningException(string.Format(NotFriend, userId, friendId));
            }
        }

        private void CheckNotAddSelfAsFiend(string userId, string friendId)
        {
            if (userId == friendId)
            {
                throw new BusinessWarningException(string.Format(AddSelfAsFiend, userId));
            }
        }
    }
}