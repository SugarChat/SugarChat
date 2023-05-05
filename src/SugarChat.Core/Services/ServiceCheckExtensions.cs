using System;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Message;

namespace SugarChat.Core.Services
{
    public static class ServiceCheckExtensions
    {
        public static void CheckNotExist(this User user)
        {
            if (user is not null)
            {
                throw new BusinessWarningException(Prompt.UserExists.WithParams(user.Id));
            }
        }

        public static void CheckExist(this User user, string id)
        {
            if (user is null || user.IsDel)
            {
                throw new BusinessWarningException(Prompt.UserNoExists.WithParams(id));
            }
        }

        public static void CheckNotExist(this Friend friend, string userId, string friendId)
        {
            if (friend is not null)
            {
                throw new BusinessWarningException(Prompt.FriendAlreadyMade.WithParams(userId, friendId));
            }
        }

        public static void CheckExist(this Friend friend, string userId, string friendId)
        {
            if (friend is null)
            {
                throw new BusinessWarningException(Prompt.NotFriend.WithParams(userId, friendId));
            }
        }

        public static void CheckNotExist(this Group group)
        {
            if (group is not null)
            {
                throw new BusinessWarningException(Prompt.GroupExists.WithParams(group.Id));
            }
        }

        public static void CheckNotAddSelfAsFiend(this User user, string userId, string friendId)
        {
            user.CheckExist(userId);
            if (user.Id == friendId)
            {
                throw new BusinessWarningException(Prompt.AddSelfAsFiend.WithParams(user.Id));
            }
        }

        public static void CheckExist(this Group group, string groupId)
        {
            if (group is null)
            {
                throw new BusinessWarningException(Prompt.GroupNoExists.WithParams(groupId));
            }
        }

        public static void CheckExist(this GroupUser groupUser, string userId, string groupId)
        {
            if (groupUser is null)
            {
                throw new BusinessWarningException(Prompt.NotInGroup.WithParams(userId, groupId));
            }
        }

        public static void CheckNotExist(this GroupUser groupUser)
        {
            if (groupUser is not null)
            {
                throw new BusinessWarningException(Prompt.GroupUserExists.WithParams(groupUser.UserId, groupUser.GroupId));
            }
        }

        public static void CheckExist(this Domain.Message message, string messageId)
        {
            if (message is null)
            {
                throw new BusinessWarningException(Prompt.MessageNoExists.WithParams(messageId));
            }
        }

        public static void CheckIsOwner(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role != UserRole.Owner)
            {
                throw new BusinessWarningException(Prompt.IsNotOwner.WithParams(userId, groupId));
            }
        }

        public static void CheckIsNotOwner(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role == UserRole.Owner)
            {
                throw new BusinessWarningException(Prompt.IsOwner.WithParams(userId, groupId));
            }
        }

        public static void CheckIsAdmin(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            //TODO : What's the logic there?
            if (groupUser.Role != UserRole.Admin && groupUser.Role != UserRole.Owner)
            {
                throw new BusinessWarningException(Prompt.NotAdmin.WithParams(userId, groupId));
            }
        }

        public static void CheckExist(this Emotion emotion, string id)
        {
            if (emotion is null)
            {
                throw new BusinessWarningException(Prompt.EmotionNotExist.WithParams(id));
            }
        }
    }
}