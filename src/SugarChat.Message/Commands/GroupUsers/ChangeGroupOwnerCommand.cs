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
        public string OwnerId { get; set; }
        public string MewOwnerId { get; set; }
        public string GroupId { get; set; }
    }
}
