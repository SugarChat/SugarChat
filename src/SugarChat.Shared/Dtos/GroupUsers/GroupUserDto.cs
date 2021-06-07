using System;
using System.Collections.Generic;

namespace SugarChat.Shared.Dtos.GroupUsers
{
    public class GroupUserDto
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string AvatarUrl { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
    }
}
