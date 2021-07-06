using Mediator.Net.Contracts;
using SugarChat.Message.Dtos;

namespace SugarChat.Message.Responses.Groups
{
    public class GetGroupProfileResponse : IResponse
    {
        public GroupDto Group { get; set; }
    }
}
