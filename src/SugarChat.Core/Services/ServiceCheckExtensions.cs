using SugarChat.Core.Basic;
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
        public const string InGroup = "User with Id {0} is member of Group with Id {1}.";
        public const string NotAdmin = "User with Id {0} is not administrator of Group with Id {1}.";
        public const string IsOwner = "User with Id {0} is owner of Group with Id {1}.";
        public const string IsNotOwner = "User with Id {0} is not owner of Group with Id {1}.";
        public const string MessageExists = "Message with Id {0} Dose not exist.";

        public static void CheckNotExist(this User user)
        {
            if (user is not null)
            {
                throw new BusinessException(StatusCode.UserAlreadyExist, string.Format(UserExists, user.Id));
            }
        }

        public static void CheckExist(this User user, string id)
        {
            if (user is null)
            {
                throw new BusinessException(StatusCode.UserDoseNotExist, string.Format(UserNoExists, id));
            }
        }

        public static void CheckNotExist(this Friend friend, string userId, string friendId)
        {
            if (friend is not null)
            {
                throw new BusinessException(StatusCode.AlreadyAddedThisFriend, string.Format(FriendAlreadyMade, userId, friendId));
            }
        }

        public static void CheckExist(this Friend friend, string userId, string friendId)
        {
            if (friend is null)
            {
                throw new BusinessException(StatusCode.NotYetFriend, string.Format(NotFriend, userId, friendId));
            }
        }

        public static void CheckNotExist(this Group group)
        {
            if (group is not null)
            {
                throw new BusinessException(StatusCode.GroupAlreadyExist, string.Format(GroupExists, group.Id));
            }
        }

        public static void CheckNotAddSelfAsFiend(this User user, string userId, string friendId)
        {
            user.CheckExist(userId);
            if (user.Id == friendId)
            {
                throw new BusinessException(StatusCode.ShouldNotAddSelfAsFriend, string.Format(AddSelfAsFiend, user.Id));
            }
        }

        public static void CheckExist(this Group group, string groupId)
        {
            if (group is null)
            {
                throw new BusinessException(StatusCode.GroupDoesNotExist, string.Format(GroupNoExists, groupId));
            }
        }

        public static void CheckExist(this GroupUser groupUser, string userId, string groupId)
        {
            if (groupUser is null)
            {
                throw new BusinessException(StatusCode.UserIsNotMemberOfGroup, string.Format(NotInGroup, userId, groupId));
            }
        }

        public static void CheckNotExist(this GroupUser groupUser, string userId, string groupId)
        {
            if (groupUser is not null)
            {
                throw new BusinessException(StatusCode.UserIsAlreadyAMemberOfTheGroup, string.Format(InGroup, userId, groupId));
            }
        }

        public static void CheckIsOwner(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role != UserRole.Owner)
            {
                throw new BusinessException(StatusCode.UserIsNotOwnerOfGroup, string.Format(IsNotOwner, userId, groupId));
            }
        }

        public static void CheckIsNotOwner(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role == UserRole.Owner)
            {
                throw new BusinessException(StatusCode.GroupOwnerNeedsToTransferTheIdentityFirst, string.Format(IsOwner, userId, groupId));
            }
        }

        public static void CheckIsAdmin(this GroupUser groupUser, string userId, string groupId)
        {
            CheckExist(groupUser, userId, groupId);
            if (groupUser.Role != UserRole.Admin && groupUser.Role != UserRole.Owner)
            {
                throw new BusinessException(StatusCode.UserIsNotAdministratorOfGroup, string.Format(NotAdmin, userId, groupId));
            }
        }

        public static void CheckExist(this Domain.Message message, string messageId)
        {
            if (message is null)
            {
                throw new BusinessException(StatusCode.MessageDoseNotExist, string.Format(MessageExists, messageId));
            }
        }
    }
}