using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.GroupMember
{
    public class AddGroupMemberCommand : ICommand
    {
        public string GroupOwnerId { get; set; }
        public string GroupId { get;set; }
        public string MemberId { get; set; }
    }
}
