using System;
using System.Collections.Generic;
using System.Text;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Command
{
    public class SendMessageCommand : ICommand
    {
        public string Content { get; set; }
    }
}
