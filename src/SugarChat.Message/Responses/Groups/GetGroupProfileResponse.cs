using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses.Groups
{
    public class GetGroupProfileResponse : IResponse
    {
        public GroupDto Result { get; set; }
    }
}
