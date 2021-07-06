using Mediator.Net.Contracts;
using SugarChat.Message.Dtos;
using System.Collections.Generic;

namespace SugarChat.Message.Responses.Conversations
{
    public class GetMessageListResponse : IResponse
    {
        public IEnumerable<MessageDto> Messages { get; set; }
        public string NextReqMessageID { get; set; }
    }
}
