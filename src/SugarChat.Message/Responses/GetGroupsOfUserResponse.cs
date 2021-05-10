using System.Collections.Generic;
using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetGroupsOfUserResponse : IResponse
    {
        public IEnumerable<GroupDto> Friends { get; set; }
    }
}