using System;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Message;

namespace SugarChat.Core.Services
{
    public static class ServiceCheckExtensions
    {
        public static void CheckNotExist(this User user)
        {
            if (user is not null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.UserExists, user.Id));
            }
        }

        public static void CheckExist(this User user, string id)
        {
            if (user is null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.UserNoExists, id));
            }
        }

        public static void CheckNotExist(this Friend friend, string userId, string friendId)
        {
            if (friend is not null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.FriendAlreadyMade, userId, friendId));
            }
        }

        public static void CheckExist(this Friend friend, string userId, string friendId)
        {
            if (friend is null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.NotFriend, userId, friendId));
            }
        }

        public static void CheckNotExist(this Group group)
        {
            if (group is not null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.GroupExists, group.Id));
            }
        }

        public static void CheckNotAddSelfAsFiend(this User user, string userId, string friendId)
        {
            user.CheckExist(userId);
            if (user.Id == friendId)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.AddSelfAsFiend, user.Id));
            }
        }

        public static void CheckExist(this Group group, string groupId)
        {
            if (group is null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.GroupNoExists, groupId));
            }
        }

        public static void CheckExist(this GroupUser groupUser, string userId, string groupId)
        {
            if (groupUser is null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.NotInGroup, userId, groupId));
            }
        }

        public static void CheckNotExist(this GroupUser groupUser)
        {
            if (groupUser is not null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.GroupUserExists, groupUser.UserId, groupUser.GroupId));
            }
        }

        public static void CheckExist(this Domain.Message message, string messageId)
        {
            if (message is null)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.MessageNoExists, messageId));
            }
        }

        public static void CheckLastReadTimeEarlierThan(this GroupUser groupUser, DateTimeOffset sentTime)
        {
            if (groupUser.LastReadTime >= sentTime)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.LastReadTimeLaterThanOrEqualTo, groupUser.UserId,
                    groupUser.GroupId, sentTime));
            }
        }

        public static void CheckIsOwner(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role != UserRole.Owner)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.IsNotOwner, userId, groupId));
            }
        }

        public static void CheckIsNotOwner(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role == UserRole.Owner)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.IsOwner, userId, groupId));
            }
        }

        public static void CheckIsAdmin(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role != UserRole.Admin && groupUser.Role != UserRole.Owner)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.NotAdmin, userId, groupId));
            }
        }
    }
}