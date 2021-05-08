using Mediator.Net.Contracts;

namespace SugarChat.Message.Events.Groups
{
    public class GroupAddedEvent : IEvent
    {
        public string Id { get; set; }
    }
}