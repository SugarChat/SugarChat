using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        UserExists = 50001,
        UserNoExists = 50002,
        FriendAlreadyMade = 50003,
        AddSelfAsFiend = 50004,
        NotFriend = 50005,
        GroupExists = 50006,
        GroupNoExists = 50007,
        NotInGroup = 50008,
        InGroup = 50009,
        NotAdmin = 50010,
        IsOwner = 50011,
        IsNotOwner = 50012,
        MessageExists = 50013,
        CustomPropertiesCanNotBeEmpty = 50014,
        TheDeletedMemberCanNotBeOwner = 50015,
        AdminCanNotDeleteAdmin = 50016,
        CanNotSetGroupMemberRoleToOwner = 50017
    }
}