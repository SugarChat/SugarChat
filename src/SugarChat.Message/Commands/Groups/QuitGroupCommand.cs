using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.Groups
{
    public class QuitGroupCommand:ICommand
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}
