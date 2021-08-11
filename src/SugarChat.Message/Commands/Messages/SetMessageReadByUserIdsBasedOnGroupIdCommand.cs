using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.Messages
{
    public class SetMessageReadByUserIdsBasedOnGroupIdCommand : ICommand
    {
        public IEnumerable<string> UserIds { get; set; }
        public string GroupId { get; set; }
    }
}
