using Mediator.Net.Contracts;
using SugarChat.Message.Dtos;

namespace SugarChat.Message.Responses
{
    public class GetUserResponse : IResponse
    {
        public UserDto User { get; set; } 
    }
}