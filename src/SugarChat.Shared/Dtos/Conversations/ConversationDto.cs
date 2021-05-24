namespace SugarChat.Shared.Dtos.Conversations
{
    public class ConversationDto
    {
        //会话Id 来自群组Id
        public string ConversationID { get; set; }
       
        //未读计数
        public int UnreadCount { get; set; }

        //会话最新的消息
        public MessageDto LastMessage { get; set; }

        //群组会话的群组资料
        public GroupDto GroupProfile { get; set; }

    }

   

}
