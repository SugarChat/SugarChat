using System.Collections.Generic;
using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetUnreadToUserFromFriendResponse : IResponse
    {
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}