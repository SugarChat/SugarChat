using System.Collections.Generic;
using Mediator.Net.Contracts;
using SugarChat.Message.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetAllUnreadToUserResponse : IResponse
    {
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}