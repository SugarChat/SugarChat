using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.Groups
{
    public class JoinGroupCommand : ICommand
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
    }
}
