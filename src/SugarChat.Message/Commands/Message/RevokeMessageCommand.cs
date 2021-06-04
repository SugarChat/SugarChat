using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.Message
{
    public class RevokeMessageCommand : ICommand
    {
        public string UserId { get; set; }
        public string MessageId { get; set; }
    }
}