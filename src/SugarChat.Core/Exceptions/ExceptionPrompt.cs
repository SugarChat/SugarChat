namespace SugarChat.Core.Exceptions
{
    public static class ExceptionPrompt
    {
        public const string UserExists = "User with Id {0} already exists.";
        public const string UserNoExists = "User with Id {0} dose not exist.";
        public const string FriendAlreadyMade = "User with Id {0} has already made friend with Id {1}.";
        public const string AddSelfAsFiend = "User with Id {0} should not add self as friend.";
        public const string NotFriend = "User with Id {0} has not been friend with Id {1} yet.";
        public const string GroupExists = "Group with Id {0} already exists.";
        public const string GroupNoExists = "Group with Id {0} dose not exist.";
        public const string NotInGroup = "User with Id {0} is not member of group with Id {1}.";
        public const string GroupUserExists = "User with Id {0} is already member of group with Id {1}.";
        public const string MessageNoExists = "Message with Id {0} dose not exist.";
        public const string NotAdmin = "User with Id {0} is not the administrator of group with Id {1}.";
        public const string IsOwner = "User with Id {0} is the owner of group with Id {1}.";
        public const string IsNotOwner = "User with Id {0} is not the owner of group with Id {1}.";

        public const string UpdateMessageFailed = "Message with Id {0} updating failed.";
        public const string AddMessageFailed = "Message with Id {0} adding failed.";
        public const string RemoveMessageFailed = "Message with Id {0} removing failed.";

        public const string LastReadTimeLaterThanOrEqualTo =
            "User with Id {0} from group with Id {1}'s Last Read Time is later than or equal to {2}.";

        public const string UpdateFriendFailed = "Friend with Id {0} updating failed.";
        public const string AddFriendFailed = "Friend with Id {0} adding failed.";
        public const string RemoveFriendFailed = "Friend with Id {0} removing failed.";

        public const string UpdateGroupUserFailed = "GroupUser with Id {0} updating failed.";
        public const string AddGroupUserFailed = "GroupUser with Id {0} adding failed.";
        public const string RemoveGroupUserFailed = "GroupUser with Id {0} removing failed.";
        
        public const string UpdateGroupFailed = "Group with Id {0} updating failed.";
        public const string AddGroupFailed = "Group with Id {0} adding failed.";
        public const string RemoveGroupFailed = "Group with Id {0} removing failed.";
        
        public const string UpdateUserFailed = "User with Id {0} updating failed.";
        public const string AddUserFailed = "User with Id {0} adding failed.";
        public const string RemoveUserFailed = "User with Id {0} removing failed.";
    }
}