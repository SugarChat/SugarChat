using System;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IRepository _repository;
        private const string UserExistsError = "User with Id {0} is already existed.";
        private const string UserNoExistsError = "User with Id {0} Dose not exist.";
        private const string FriendAlreadyMadeError = "User with Id {0} has already made friend with Id {1}.";
        private const string NotFriendError = "User with Id {0} is not friend with Id {1} yet.";

        public UserService(IRepository repository)
        {
            _repository = repository;
        }


        public virtual void Add(User user)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task AddAsync(User user)
        {
            if (await _repository.AnyAsync<User>(o => o.Id == user.Id))
            {
                throw new BusinessException(string.Format(UserExistsError, user.Id));
            }

            await _repository.AddAsync(user, new());
        }

        public virtual void Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task DeleteAsync(string id)
        {
            User user = await GetAsync(id);
            await _repository.RemoveAsync(user, new());
        }

        public User Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<User> GetAsync(string id)
        {
            User user = await _repository.SingleAsync<User>(o => o.Id == id);
            if (user is null)
            {
                throw new BusinessException(string.Format(UserNoExistsError, id));
            }

            return user;
        }

        public virtual void AddFriend(string userId, string friendId)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task AddFriendAsync(string userId, string friendId)
        {
            await CheckExist(userId);
            await CheckExist(friendId);
            await CheckFriend(userId, friendId);
            Friend friendEntity = new Friend
            {
                UserId = userId,
                FriendId = friendId,
                BecomeFriendAt = new(DateTime.UtcNow)
            };
            await _repository.AddAsync(friendEntity, new CancellationToken());
            
            async Task CheckFriend(string userId, string friendId)
            {
                if (!await _repository.AnyAsync<Friend>(o => o.UserId == userId && o.FriendId == friendId))
                {
                    throw new BusinessException(string.Format(FriendAlreadyMadeError, userId, friendId));
                }
            }
        }


        public virtual void RemoveFriend(string userId, string friendId)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task RemoveFriendAsync(string userId, string friendId)
        {
            await CheckExist(userId);
            await CheckExist(friendId);
            await CheckNotFriend(userId, friendId);
            
            async Task CheckNotFriend(string userId, string friendId)
            {
                if (await _repository.AnyAsync<Friend>(o => o.UserId == userId && o.FriendId == friendId))
                {
                    throw new BusinessException(string.Format(NotFriendError, userId, friendId));
                }
            }
        }

        private async Task CheckExist(string id)
        {
            if (!await _repository.AnyAsync<User>(o => o.Id == id))
            {
                throw new BusinessException(string.Format(UserExistsError, id));
            }
        }
    }
}