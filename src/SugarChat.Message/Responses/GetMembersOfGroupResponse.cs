using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos.GroupUsers;
using System.Collections.Generic;

namespace SugarChat.Message.Responses
{
    public class GetMembersOfGroupResponse : IResponse
    {
        public IEnumerable<GroupUserDto> Members { get; set; }
    }
}