using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.GroupUsers
{
    public class ChangeGroupOwnerCommand : ICommand
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string GroupId { get; set; }
    }
}
