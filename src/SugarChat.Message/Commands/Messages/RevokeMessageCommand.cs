using System;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Messages
{
    public class RevokeMessageCommand : ICommand
    {
        public string UserId { get; set; }
        public string MessageId { get; set; }
        public TimeSpan RevokeTimeLimit { get; set; } = TimeSpan.FromMinutes(2);
    }
}