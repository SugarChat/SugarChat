using System.Collections.Generic;

namespace SugarChat.Shared.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
    }
}