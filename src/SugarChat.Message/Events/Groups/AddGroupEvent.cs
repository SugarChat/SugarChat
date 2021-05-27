using Mediator.Net.Contracts;

namespace SugarChat.Message.Events.Groups
{
    public class AddGroupEvent : EventBase
    {
        public string Id { get; set; }
    }
}