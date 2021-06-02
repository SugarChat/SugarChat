using System.Collections.Generic;

namespace SugarChat.Shared.Dtos
{
    public class GroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
        public int MemberCount { get; set; }
    }
}