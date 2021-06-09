using System.Collections.Generic;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetMessagesOfGroupBeforeResponse
    {
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}