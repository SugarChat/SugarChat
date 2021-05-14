using Mediator.Net.Contracts;

namespace SugarChat.Message.Events.Groups
{
    public class GroupAddedEvent : EventBase
    {
        public string Id { get; set; }
    }
}