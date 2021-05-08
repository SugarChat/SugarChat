using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Friends;
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
        private readonly IRepository _repository;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IFriendDataProvider _friendDataProvider;

        //TODO Error const would be moved to specific class or json file
        private const string UserExistsError = "User with Id {0} is already existed.";
        private const string UserNoExistsError = "User with Id {0} Dose not exist.";
        private const string FriendAlreadyMadeError = "User with Id {0} has already made friend with Id {1}.";
        private const string AddSelfAsFiendError = "User with Id {0} Should not add self as friend.";
        private const string NotFriendError = "User with Id {0} is not friend with Id {1} yet.";

        public UserService(IMapper mapper, IRepository repository, IUserDataProvider userDataProvider,
            IFriendDataProvider friendDataProvider)
        {
            _mapper = mapper;
            _repository = repository;
            _userDataProvider = userDataProvider;
            _friendDataProvider = friendDataProvider;
        }


        public async Task<UserAddedEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellation);
            if (user is not null)
            {
                return new UserAddedEvent
                {
                    Id = command.Id,
                    Status = EventStatus.Failed,
                    Infomation = new BusinessException(String.Format(UserExistsError, command.Id))
                };
            }

            user = _mapper.Map<User>(command);

            await _repository.AddAsync(user, cancellation).ConfigureAwait(false);
            await _repository.SaveChangesAsync(cancellation).ConfigureAwait(false);

            return new UserAddedEvent
            {
                Id = user.Id,
                Status = EventStatus.Success,
            };
        }

        public async Task<UserDeletedEvent> DeleteUserAsync(DeleteUserCommand command,
            CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellation);
            if (user is null)
            {
                return new UserDeletedEvent
                {
                    Id = command.Id,
                    Status = EventStatus.Failed,
                    Infomation = new BusinessException(String.Format(UserNoExistsError, command.Id))
                };
            }

            await _repository.RemoveAsync(user, cancellation).ConfigureAwait(false);
            await _repository.SaveChangesAsync(cancellation).ConfigureAwait(false);

            return new UserDeletedEvent
            {
                Id = command.Id,
                Status = EventStatus.Success,
            };
        }

        public async Task<FriendAddedEvent> AddFriendAsync(AddFriendCommand command,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(command.UserId, cancellation);
            if (user is null)
            {
                return new FriendAddedEvent
                {
                    Id = null,
                    Status = EventStatus.Failed,
                    Infomation = new BusinessException(String.Format(UserNoExistsError, command.UserId))
                };
            }

            if (command.UserId == command.FriendId)
            {
                return new FriendAddedEvent
                {
                    Id = null,
                    Status = EventStatus.Failed,
                    Infomation = new BusinessException(String.Format(AddSelfAsFiendError, command.UserId))
                };
            }

            User friend = await GetUserAsync(command.FriendId, cancellation);
            if (friend is null)
            {
                return new FriendAddedEvent
                {
                    Id = null,
                    Status = EventStatus.Failed,
                    Infomation = new BusinessException(String.Format(UserNoExistsError, command.FriendId))
                };
            }

            Friend existFriend =
                await _friendDataProvider.GetByUsersIdAsync(command.UserId, command.FriendId, cancellation);
            if (existFriend is not null)
            {
                return new FriendAddedEvent
                {
                    Id = null,
                    Status = EventStatus.Failed,
                    Infomation =
                        new BusinessException(String.Format(FriendAlreadyMadeError, command.UserId, command.FriendId))
                };
            }

            Friend makeFriend = new Friend
            {
                UserId = command.UserId,
                FriendId = command.FriendId,
                BecomeFriendAt = DateTimeOffset.UtcNow
            };

            await _repository.AddAsync(makeFriend, cancellation).ConfigureAwait(false);
            await _repository.SaveChangesAsync(cancellation).ConfigureAwait(false);

            return new FriendAddedEvent
            {
                Id = user.Id,
                Status = EventStatus.Success,
            };
        }

        public async Task<FriendRemovedEvent> RemoveFriendAsync(RemoveFriendCommand command,
            CancellationToken cancellation = default)
        {
            Friend friend = await _friendDataProvider.GetByUsersIdAsync(command.UserId, command.FriendId, cancellation);
            if (friend is null)
            {
                return new FriendRemovedEvent
                {
                    UserId = command.UserId,
                    FriendId = command.FriendId,
                    Status = EventStatus.Failed,
                    Infomation = new BusinessException(String.Format(NotFriendError, command.UserId, command.FriendId))
                };
            }

            await _repository.RemoveAsync(friend, cancellation).ConfigureAwait(false);
            await _repository.SaveChangesAsync(cancellation).ConfigureAwait(false);

            return new FriendRemovedEvent
            {
                UserId = command.UserId,
                FriendId = command.FriendId,
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

            IEnumerable<User> friends = await
                _userDataProvider.GetRangeByIdAsync(_repository.Query<Friend>().Where(o => o.UserId == request.Id)
                    .Select(o => o.FriendId), cancellation);

            IEnumerable<UserDto> friendsDto = _mapper.Map<IEnumerable<UserDto>>(friends);
            return new GetFriendsOfUserResponse
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