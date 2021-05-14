using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;

namespace SugarChat.Core.Services
{
    public static class ServiceCheckExtensions
    {
        private const string UserExists = "User with Id {0} already exists.";
        private const string UserNoExists = "User with Id {0} Dose not exist.";
        private const string FriendAlreadyMade = "User with Id {0} has already made friend with Id {1}.";
        private const string AddSelfAsFiend = "User with Id {0} Should not add self as friend.";
        private const string NotFriend = "User with Id {0} has not been friend with Id {1} yet.";
        private const string GroupExists = "Group with Id {0} already exists.";

        public static void CheckNotExist(this User user)
        {
            if (user is not null)
            {
                throw new BusinessWarningException(string.Format(UserExists, user.Id));
            }
        }

        public static void CheckExist(this User user, string id)
        {
            if (user is null)
            {
                throw new BusinessWarningException(string.Format(UserNoExists, id));
            }
        }

        public static void CheckNotExist(this Friend friend, string userId, string friendId)
        {
            if (friend is not null)
            {
                throw new BusinessWarningException(string.Format(FriendAlreadyMade, userId, friendId));
            }
        }

        public static void CheckExist(this Friend friend, string userId, string friendId)
        {
            if (friend is null)
            {
                throw new BusinessWarningException(string.Format(NotFriend, userId, friendId));
            }
        }
        
        public static void CheckNotExist(this Group group)
        {
            if (group is not null)
            {
                throw new BusinessWarningException(string.Format(GroupExists, group.Id));
            }
        }

        public static void CheckNotAddSelfAsFiend(this User user, string userId, string friendId)
        {
            user.CheckExist(userId);
            if (user.Id == friendId)
            {
                throw new BusinessWarningException(string.Format(AddSelfAsFiend, user.Id));
            }
        }
    }
}