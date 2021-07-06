using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.GroupUsers
{
    public class AddGroupMemberCommand : ICommand
    {
        public string GroupId { get; set; }
        public string AdminId { get; set; }
        public IEnumerable<string> GroupUserIds { get; set; }
        public UserRole Role { set; get; }
    }
}
