using System.Collections.Generic;
using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetAllHistoryToUserResponse : IResponse
    {
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}