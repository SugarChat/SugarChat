using System.Collections.Generic;
using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;
using SugarChat.Shared.Paging;

namespace SugarChat.Message.Responses
{
    public class GetFriendsOfUserResponse : IResponse
    {
        public PagedResult<UserDto> Friends { get; set; }
    }
}