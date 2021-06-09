using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetCurrentUserResponse : IResponse
    {
        public UserDto User { get; set; } 
    }
}