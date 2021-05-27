using System.Collections.Generic;
using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetGroupMembersResponse : IResponse
    {
        public IEnumerable<UserDto> Members { get; set; }
    }
}