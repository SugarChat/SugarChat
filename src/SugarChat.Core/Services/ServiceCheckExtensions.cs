using System;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Message;

namespace SugarChat.Core.Services
{
    public static class ServiceCheckExtensions
    {
        public const string UserExists = "User with Id {0} already exists.";
        public const string UserNoExists = "User with Id {0} Dose not exist.";
        public const string FriendAlreadyMade = "User with Id {0} has already made friend with Id {1}.";
        public const string AddSelfAsFiend = "User with Id {0} Should not add self as friend.";
        public const string NotFriend = "User with Id {0} has not been friend with Id {1} yet.";
        public const string GroupExists = "Group with Id {0} already exists.";
        public const string GroupNoExists = "Group with Id {0} Dose not exist.";
        public const string NotInGroup = "User with Id {0} is not member of Group with Id {1}.";
        public const string GroupUserExists = "User with Id {0} is already member of Group with Id {1}.";
        public const string MessageNoExists = "Message with Id {0} Dose not exist.";
        public const string NotAdmin = "User with Id {0} is not administrator of Group with Id {1}.";
        public const string IsOwner = "User with Id {0} is owner of Group with Id {1}.";
        public const string IsNotOwner = "User with Id {0} is not owner of Group with Id {1}.";
        public const string MessageExists = "Message with Id {0} Dose not exist.";

        private const string LastReadTimeLaterThanOrEqualTo =
            "User with Id {0} From Group with Id {1}'s Last Read Timed is later than or equal to {2}";

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

        public static void CheckExist(this Group group, string groupId)
        {
            if (group is null)
            {
                throw new BusinessWarningException(string.Format(GroupNoExists, groupId));
            }
        }

        public static void CheckExist(this GroupUser groupUser, string userId, string groupId)
        {
            if (groupUser is null)
            {
                throw new BusinessWarningException(string.Format(NotInGroup, userId, groupId));
            }
        }

        public static void CheckNotExist(this GroupUser groupUser)
        {
            if (groupUser is not null)
            {
                throw new BusinessWarningException(string.Format(GroupUserExists, groupUser.UserId, groupUser.GroupId));
            }
        }

        public static void CheckExist(this Domain.Message message, string messageId)
        {
            if (message is null)
            {
                throw new BusinessWarningException(string.Format(MessageNoExists, messageId));
            }
        }

        public static void CheckLastReadTimeEarlierThan(this GroupUser groupUser, DateTimeOffset sentTime)
        {
            if (groupUser.LastReadTime >= sentTime)
            {
                throw new BusinessWarningException(string.Format(LastReadTimeLaterThanOrEqualTo, groupUser.UserId,
                    groupUser.GroupId, sentTime));
            }
        }

        public static void CheckIsOwner(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role != UserRole.Owner)
            {
                throw new BusinessWarningException(string.Format(IsNotOwner, userId, groupId));
            }
        }

        public static void CheckIsNotOwner(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role == UserRole.Owner)
            {
                throw new BusinessWarningException(string.Format(IsOwner, userId, groupId));
            }
        }

        public static void CheckIsAdmin(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role != UserRole.Admin && groupUser.Role != UserRole.Owner)
            {
                throw new BusinessWarningException(string.Format(NotAdmin, userId, groupId));
            }
        }
    }
}