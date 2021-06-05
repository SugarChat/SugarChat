namespace SugarChat.Core.Basic
{
    public class SugarChatResponse<T> : ISugarChatResponse<T>
    {
        public StatusCode Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class SugarChatResponse : ISugarChatResponse
    {
        public StatusCode Code { get; set; }
        public string Message { get; set; }
    }
    public enum StatusCode
    {
        Ok = 20000,
        UserAlreadyExist = 50001,
        UserDoseNotExist = 50002,
        AlreadyAddedThisFriend = 50003,
        ShouldNotAddSelfAsFriend = 50004,
        NotYetFriend = 50005,
        GroupAlreadyExist = 50006,
        GroupDoesNotExist = 50007,
        UserIsNotMemberOfGroup = 50008,
        UserIsAlreadyAMemberOfTheGroup = 50009,
        UserIsNotAdministratorOfGroup = 50010,
        GroupOwnerNeedsToTransferTheIdentityFirst = 50011,
        UserIsNotOwnerOfGroup = 50012,
        MessageDoseNotExist = 50013,
        CustomPropertiesCanNotBeEmpty = 50014,
        TheDeletedMemberCanNotBeOwner = 50015,
        AdminCanNotDeleteAdmin = 50016,
        CanNotSetGroupMemberRoleToOwner = 50017
    }
}