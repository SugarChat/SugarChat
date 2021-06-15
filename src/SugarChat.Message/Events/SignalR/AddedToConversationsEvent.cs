using System;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Events.SignalR
{
    public class AddedToConversationsEvent : IEvent
    {
        public string UserId { get; set; }
    }
}