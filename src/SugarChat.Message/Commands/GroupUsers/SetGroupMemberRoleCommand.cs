using Mediator.Net.Contracts;
using SugarChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.GroupUsers
{
    public class SetGroupMemberRoleCommand : ICommand
    {
        public string GroupId { get; set; }
        public string OwnerId { get; set; }
        public string MemberId { get; set; }
        public UserRole Role { get; set; }
    }
}
