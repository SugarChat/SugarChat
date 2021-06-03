namespace SugarChat.Message.Events.Groups
{
    public class GroupProfileUpdatedEvent : EventBase
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public string Description { get; set; }
    }
}
