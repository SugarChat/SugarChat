using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos.Conversations;
using System.Collections.Generic;

namespace SugarChat.Message.Responses.Conversations
{
    public class GetConversationListResponse : IResponse
    {
        public IEnumerable<ConversationDto> Result { get; set; }
    }
}
