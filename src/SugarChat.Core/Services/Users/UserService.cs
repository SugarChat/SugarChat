using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Groups;
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

        public UserService(IMapper mapper, IRepository repository, IUserDataProvider userDataProvider)
        {
            _mapper = mapper;
            _repository = repository;
            _userDataProvider = userDataProvider;
        }


        public async Task<UserAddedEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellation = default)
        {
            var user = _mapper.Map<User>(command);
            try
            {
                await _repository.AddAsync(user, cancellation).ConfigureAwait(false);
                await _repository.SaveChangesAsync(cancellation).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return new UserAddedEvent
                {
                    Id = user.Id,
                    Status = EventStatus.Failed,
                    Infomation = e
                };
            }

            return new UserAddedEvent
            {
                Id = user.Id,
                Status = EventStatus.Success,
            };
        }

        public async Task<UserDeletedEvent> DeleteUserAsync(DeleteUserCommand command,
            CancellationToken cancellation = default)
        {
            try
            {
                User user = await _repository.SingleAsync<User>(o => o.Id == command.Id);
                await _repository.RemoveAsync(user, cancellation).ConfigureAwait(false);
                await _repository.SaveChangesAsync(cancellation).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return new UserDeletedEvent()
                {
                    Id = command.Id,
                    Status = EventStatus.Failed,
                    Infomation = e
                };
            }

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
                    Infomation = new BusinessException("")
                };
            }
            throw new NotImplementedException();
        }

        public Task<FriendRemovedEvent> RemoveFriendAsync(RemoveFriendCommand command,
            CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<GetUserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
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