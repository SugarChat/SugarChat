using Mediator.Net.Contracts;

namespace SugarChat.Message.Events.Groups
{
    public class GroupAddedEvent : EventBase
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public string AvatarUrl { get; set; }
        
        public string Description { get; set; }
    }
}