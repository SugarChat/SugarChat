using System.Collections.Generic;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetMessagesOfGroupResponse
    {
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}