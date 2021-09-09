using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using System.Collections.Generic;

namespace SugarChat.Message.Responses
{
    public class GetMessagesOfGroupResponse
    {
        public PagedResult<MessageDto> Messages { get; set; }
    }
}