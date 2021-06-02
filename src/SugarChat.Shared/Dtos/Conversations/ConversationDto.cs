namespace SugarChat.Shared.Dtos.Conversations
{
    public class ConversationDto
    {        
        public string ConversationID { get; set; }       
        public ConversationType Type { get; set; }       
        public int UnreadCount { get; set; }       
        public MessageDto LastMessage { get; set; }       
        public GroupDto GroupProfile { get; set; }
    }

    public enum ConversationType
    {
        Group,
        C2C,
        System
    }



}
