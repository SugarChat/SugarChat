using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Common
{
    public enum ExceptionCode : int
    {
        Unknown = 50000,
        
        #region User
        UserExists = 50001,
        UserNoExists = 50002,
        UpdateUserFailed = 50003,
        AddUserFailed = 50004,
        RemoveUserFailed = 50005,
        NotAllUsersExists = 50006,
        #endregion
        
        #region Group
        GroupExists = 50501,
        GroupNoExists = 50502,
        NotInGroup = 50503,
        GroupUserExists = 50504,
        NotAdmin = 50505,
        IsOwner = 50506,
        IsNotOwner = 50507,
        UpdateGroupFailed = 50508,
        AddGroupFailed = 50509,
        RemoveGroupFailed = 50510,
        #endregion

        #region Friend
        FriendAlreadyMade = 51001,
        AddSelfAsFiend = 51002,
        NotFriend = 51003,
        UpdateFriendFailed = 51004,
        AddFriendFailed = 51005,
        RemoveFriendFailed = 51006,
        #endregion

        #region Message
        MessageNoExists = 51501,
        UpdateMessageFailed = 51502,
        AddMessageFailed = 51503,
        RemoveMessageFailed = 51504,
        LastReadTimeLaterThanOrEqualTo = 51505,
        RevokeOthersMessage = 51506,
        TooLateToRevoke = 51507,
        #endregion
        
        #region GroupUser
        UpdateGroupUserFailed = 52001,
        AddGroupUserFailed = 52002,
        RemoveGroupUserFailed = 52003,
        AddGroupUsersFailed = 52004,
        RemoveGroupUsersFailed = 52005,
        NoCustomProperty = 52006,
        SameGroupUser = 52007,
        SomeGroupUsersExist = 52008,
        NotAllGroupUsersExist = 52009,
        RemoveOwnerFromGroup = 52010,
        RemoveAdminByAdmin = 52011,
        SetGroupOwner = 52012,
        AddUsersToWrongGroup = 52013,
        UpdateGroupUsersFailed = 52014,
        #endregion

        #region Global

        IdIsNullOrEmpty = 52501

        #endregion
        
    }
}
