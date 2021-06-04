using Mediator.Net.Contracts;
using System.Collections.Generic;

namespace SugarChat.Message.Commands.GroupUsers
{
    public class SetGroupMemberCustomFieldCommand : ICommand
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }


    }
}
