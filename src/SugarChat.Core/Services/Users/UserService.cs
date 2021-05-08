using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Commands.Users;
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


        public async Task<UserAddedEvent> AddUserAsync(AddUserCommand command, CancellationToken cancellation)
        {
            var user = _mapper.Map<User>(command);

            await _repository.AddAsync(user, cancellation).ConfigureAwait(false);
            await _repository.SaveChangesAsync(cancellation).ConfigureAwait(false);

            return new UserAddedEvent
            {
                Id = user.Id
            };
        }

        public Task<UserDeletedEvent> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<FriendAddedEvent> AddFriendAsync(AddFriendCommand command, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<FriendRemovedEvent> RemoveFriendAsync(RemoveFriendCommand command, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<GetUserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<GetUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request, CancellationToken cancellation)
        {
            return Task.FromResult(new GetUserResponse{User = new UserDto()});
        }
    }
}