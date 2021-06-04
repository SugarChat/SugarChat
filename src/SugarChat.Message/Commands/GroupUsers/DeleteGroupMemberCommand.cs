using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.GroupUsers
{
    public class DeleteGroupMemberCommand : ICommand
    {
        public string GroupId { get; set; }
        public string AdminId { get; set; }
        public List<string> UserIdList { get; set; }
    }
}
