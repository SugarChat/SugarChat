using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Friends;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using SugarChat.Core.Services.GroupUsers;

namespace SugarChat.Core.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IFriendDataProvider _friendDataProvider;
        private readonly ITransactionManager _transactionManagement;
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public UserService(IMapper mapper,
            IUserDataProvider userDataProvider,
            IFriendDataProvider friendDataProvider,
            ITransactionManager transactionManagement,
            IGroupUserDataProvider groupUserDataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _friendDataProvider = friendDataProvider;
            _transactionManagement = transactionManagement;
            _groupUserDataProvider = groupUserDataProvider;
        }

        public async Task<UserAddedEvent> AddUserAsync(AddUserCommand command,
            CancellationToken cancellationToken = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
            user.CheckNotExist();

            user = _mapper.Map<User>(command);
            await _userDataProvider.AddAsync(user, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<UserAddedEvent>(command);
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

            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await _userDataProvider.RemoveAsync(user, cancellationToken).ConfigureAwait(false);
                    await _groupUserDataProvider.RemoveRangeAsync(x => x.UserId == user.Id, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }

            return _mapper.Map<UserRemovedEvent>(command);
        }

        public async Task<GetUserResponse> GetUserAsync(GetUserRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await _userDataProvider.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
            user.CheckExist(request.Id);
            return new()
            {
                User = _mapper.Map<UserDto>(await _userDataProvider.GetByIdAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false))
            };
        }

        public Task<GetCurrentUserResponse> GetCurrentUserAsync(GetCurrentUserRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new GetCurrentUserResponse { User = new UserDto() });
        }


        public async Task<GetFriendsOfUserResponse> GetFriendsOfUserAsync(GetFriendsOfUserRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.Id, cancellationToken).ConfigureAwait(false);
            user.CheckExist(request.Id);

            PagedResult<Friend> friends = await _friendDataProvider.GetAllFriendsByUserIdAsync(request.Id,
                request.PageSettings, cancellationToken).ConfigureAwait(false);
            IEnumerable<User> users = await
                _userDataProvider.GetRangeByIdAsync(friends.Result.Select(o => o.FriendId), cancellationToken)
                    .ConfigureAwait(false);

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

        public async Task<UsersBatchAddedEvent> BatchAddUsersAsync(BatchAddUsersCommand command, CancellationToken cancellationToken = default)
        {
            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var users = _mapper.Map<IEnumerable<User>>(command.Users);
                    await _userDataProvider.RemoveRangeAsync(users, cancellationToken).ConfigureAwait(false);
                    await _userDataProvider.AddRangeAsync(users, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }

            return _mapper.Map<UsersBatchAddedEvent>(command);
        }
    }
}