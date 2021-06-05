namespace SugarChat.Core.Exceptions
{
    public static class ExceptionPrompt
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

        public const string UpdateMessageFailed = "Message with Id {0} Update Failed.";
        public const string AddMessageFailed = "Message with Id {0} Add Failed.";
        public const string RemoveMessageFailed = "Message with Id {0} Remove Failed.";

        public const string LastReadTimeLaterThanOrEqualTo =
            "User with Id {0} From Group with Id {1}'s Last Read Timed is later than or equal to {2}";

        public const string UpdateFriendFailed = "Friend with Id {0} Update Failed.";
        public const string AddFriendFailed = "Friend with Id {0} Add Failed.";
        public const string RemoveFriendFailed = "Friend with Id {0} Remove Failed.";

        public const string UpdateGroupUserFailed = "GroupUser with Id {0} Update Failed.";
        public const string AddGroupUserFailed = "GroupUser with Id {0} Add Failed.";
        public const string RemoveGroupUserFailed = "GroupUser with Id {0} Remove Failed.";
        
        public const string UpdateGroupFailed = "Group with Id {0} Update Failed.";
        public const string AddGroupFailed = "Group with Id {0} Add Failed.";
        public const string RemoveGroupFailed = "Group with Id {0} Remove Failed.";
        
        public const string UpdateUserFailed = "User with Id {0} Update Failed.";
        public const string AddUserFailed = "User with Id {0} Add Failed.";
        public const string RemoveUserFailed = "User with Id {0} Remove Failed.";
    }
}