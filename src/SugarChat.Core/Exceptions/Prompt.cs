namespace SugarChat.Core.Exceptions
{
    public static class Prompt
    {
        #region User

        public static readonly ExceptionPrompt UserExists =
            new(1000, "User with Id {0} already exists.");

        public static readonly ExceptionPrompt UserNoExists =
            new(1010, "User with Id {0} dose not exist.");

        public static readonly ExceptionPrompt UpdateUserFailed =
            new(1020, "User with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddUserFailed =
            new(1030, "User with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveUserFailed =
            new(1040, "User with Id {0} removing failed.");

        #endregion

        #region Friend

        public static readonly ExceptionPrompt FriendAlreadyMade =
            new(2000, "User with Id {0} has already made friend with Id {1}.");

        public static readonly ExceptionPrompt AddSelfAsFiend =
            new(2010, "User with Id {0} should not add self as friend.");

        public static readonly ExceptionPrompt NotFriend =
            new(2020, "User with Id {0} has not been friend with Id {1} yet.");

        public static readonly ExceptionPrompt UpdateFriendFailed =
            new(2030, "Friend with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddFriendFailed =
            new(2040, "Friend with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveFriendFailed =
            new(2050, "Friend with Id {0} removing failed.");

        #endregion

        #region Group

        public static readonly ExceptionPrompt GroupExists =
            new(3000, "Group with Id {0} already exists.");

        public static readonly ExceptionPrompt GroupNoExists =
            new(3010, "Group with Id {0} dose not exist.");

        public static readonly ExceptionPrompt NotInGroup =
            new(3020, "User with Id {0} is not member of group with Id {1}.");

        public static readonly ExceptionPrompt GroupUserExists =
            new(3030, "User with Id {0} is already member of group with Id {1}.");

        public static readonly ExceptionPrompt NotAdmin =
            new(3040, "User with Id {0} is not the administrator of group with Id {1}.");

        public static readonly ExceptionPrompt IsOwner =
            new(3050, "User with Id {0} is the owner of group with Id {1}.");

        public static readonly ExceptionPrompt IsNotOwner =
            new(3060, "User with Id {0} is not the owner of group with Id {1}.");

        public static readonly ExceptionPrompt UpdateGroupFailed =
            new(3070, "Group with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddGroupFailed =
            new(3080, "Group with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveGroupFailed =
            new(3090, "Group with Id {0} removing failed.");

        #endregion


        #region Message

        public static readonly ExceptionPrompt MessageNoExists =
            new(4000, "Message with Id {0} dose not exist.");

        public static readonly ExceptionPrompt UpdateMessageFailed =
            new(4010, "Message with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddMessageFailed = 
            new(4020, "Message with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveMessageFailed =
            new(4030, "Message with Id {0} removing failed.");

        public static readonly ExceptionPrompt LastReadTimeLaterThanOrEqualTo =
            new(4040, "User with Id {0} from group with Id {1}'s Last Read Time is later than or equal to {2}.");

        #endregion

        #region GroupUser

        public static readonly ExceptionPrompt UpdateGroupUserFailed =
            new(5000, "GroupUser with Id {0} updating failed.");

        public static readonly ExceptionPrompt AddGroupUserFailed =
            new(5020, "GroupUser with Id {0} adding failed.");

        public static readonly ExceptionPrompt RemoveGroupUserFailed =
            new(5030, "GroupUser with Id {0} removing failed.");

        #endregion
    }
}