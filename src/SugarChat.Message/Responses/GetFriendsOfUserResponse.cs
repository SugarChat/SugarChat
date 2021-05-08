using System.Collections.Generic;
using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetFriendsOfUserResponse : IResponse
    {
        public IEnumerable<UserDto> Friends { get; set; }
    }
}