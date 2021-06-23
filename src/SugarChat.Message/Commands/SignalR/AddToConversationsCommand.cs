using System;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.SignalR
{
    public class AddToConversationsCommand : ICommand
    {
        public string UserId { get; set; }
    }
}