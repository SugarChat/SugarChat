using SugarChat.Core.Common;

namespace SugarChat.Core.Exceptions
{
    public static class Prompt
    {
        #region User

        public static readonly ExceptionPrompt UserExists =
            new(ExceptionCode.UserExists, "User with Id {0} already exists.");

        public static readonly ExceptionPrompt UserNoExists =
            new(ExceptionCode.UserNoExists, "User with Id {0} dose not exist.");

        public static readonly ExceptionPrompt UpdateUserFailed =
            new(ExceptionCode.UpdateUserFailed, "User with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddUserFailed =
            new(ExceptionCode.AddUserFailed, "User with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveUserFailed =
            new(ExceptionCode.RemoveUserFailed, "User with Id {0} removing failed.");  
        
        public static readonly ExceptionPrompt NotAllUsersExists =
            new(ExceptionCode.NotAllUsersExists, "Not all users exist.");
        
        #endregion

        #region Friend

        public static readonly ExceptionPrompt FriendAlreadyMade =
            new(ExceptionCode.FriendAlreadyMade, "User with Id {0} has already made friend with Id {1}.");

        public static readonly ExceptionPrompt AddSelfAsFiend =
            new(ExceptionCode.AddSelfAsFiend, "User with Id {0} should not add self as friend.");

        public static readonly ExceptionPrompt NotFriend =
            new(ExceptionCode.NotFriend, "User with Id {0} has not been friend with Id {1} yet.");

        public static readonly ExceptionPrompt UpdateFriendFailed =
            new(ExceptionCode.UpdateFriendFailed, "Friend with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddFriendFailed =
            new(ExceptionCode.AddFriendFailed, "Friend with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveFriendFailed =
            new(ExceptionCode.RemoveFriendFailed, "Friend with Id {0} removing failed.");

        #endregion

        #region Group

        public static readonly ExceptionPrompt GroupExists =
            new(ExceptionCode.GroupExists, "Group with Id {0} already exists.");

        public static readonly ExceptionPrompt GroupNoExists =
            new(ExceptionCode.GroupNoExists, "Group with Id {0} dose not exist.");

        public static readonly ExceptionPrompt NotInGroup =
            new(ExceptionCode.NotInGroup, "User with Id {0} is not member of group with Id {1}.");

        public static readonly ExceptionPrompt GroupUserExists =
            new(ExceptionCode.GroupUserExists, "User with Id {0} is already member of group with Id {1}.");

        public static readonly ExceptionPrompt NotAdmin =
            new(ExceptionCode.NotAdmin, "User with Id {0} is not the administrator of group with Id {1}.");

        public static readonly ExceptionPrompt IsOwner =
            new(ExceptionCode.IsOwner, "User with Id {0} is the owner of group with Id {1}.");

        public static readonly ExceptionPrompt IsNotOwner =
            new(ExceptionCode.IsNotOwner, "User with Id {0} is not the owner of group with Id {1}.");

        public static readonly ExceptionPrompt UpdateGroupFailed =
            new(ExceptionCode.UpdateGroupFailed, "Group with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddGroupFailed =
            new(ExceptionCode.AddGroupFailed, "Group with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveGroupFailed =
            new(ExceptionCode.RemoveGroupFailed, "Group with Id {0} removing failed.");

        #endregion


        #region Message

        public static readonly ExceptionPrompt MessageNoExists =
            new(ExceptionCode.MessageNoExists, "Message with Id {0} dose not exist.");

        public static readonly ExceptionPrompt UpdateMessageFailed =
            new(ExceptionCode.UpdateMessageFailed, "Message with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddMessageFailed =
            new(ExceptionCode.AddMessageFailed, "Message with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveMessageFailed =
            new(ExceptionCode.RemoveMessageFailed, "Message with Id {0} removing failed.");

        public static readonly ExceptionPrompt LastReadTimeLaterThanOrEqualTo =
            new(ExceptionCode.LastReadTimeLaterThanOrEqualTo,
                "User with Id {0} from group with Id {1}'s Last Read Time is later than or equal to {2}.");

        #endregion

        #region GroupUser

        public static readonly ExceptionPrompt UpdateGroupUserFailed =
            new(ExceptionCode.UpdateGroupUserFailed, "GroupUser with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddGroupUserFailed =
            new(ExceptionCode.AddGroupUserFailed, "GroupUser with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveGroupUserFailed =
            new(ExceptionCode.RemoveGroupUserFailed, "GroupUser with Id {0} removing failed.");

        public static readonly ExceptionPrompt AddGroupUsersFailed =
            new(ExceptionCode.AddGroupUsersFailed, "Adding {0} groupUsers failed, only {1} of them added.");

        public static readonly ExceptionPrompt RemoveGroupUsersFailed =
            new(ExceptionCode.RemoveGroupUsersFailed, "Removing {0} groupUsers failed, only {1} of them removed.");

        public static readonly ExceptionPrompt NoCustomProperty =
            new(ExceptionCode.NoCustomProperty, "There is no custom property provided.");
        
        public static readonly ExceptionPrompt SameGroupUser =
            new(ExceptionCode.SameGroupUser, "Both GroupUsers have the same Id.");  
        
        public static readonly ExceptionPrompt SomeGroupUsersExist =
            new(ExceptionCode.SomeGroupUsersExist, "Some of the groupUsers are already exist.");
        

        #endregion


    }
}