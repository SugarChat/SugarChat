using SugarChat.Message.Dtos;
using System.Collections.Generic;

namespace SugarChat.Message.Responses
{
    public class GetMessagesOfGroupResponse
    {
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}