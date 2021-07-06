using System.Collections.Generic;
using Mediator.Net.Contracts;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;

namespace SugarChat.Message.Responses
{
    public class GetFriendsOfUserResponse : IResponse
    {
        public PagedResult<UserDto> Friends { get; set; }
    }
}