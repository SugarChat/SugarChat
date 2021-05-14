using System.Collections.Generic;

namespace SugarChat.Message.Commands.Users
{
    public class UpdateUserCommand: ICommand
    {
        public string Id { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
    }
}