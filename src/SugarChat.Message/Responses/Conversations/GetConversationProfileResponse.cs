using Mediator.Net.Contracts;
using SugarChat.Shared.Dtos;

namespace SugarChat.Message.Responses.Conversations
{
    public class GetConversationProfileResponse : IResponse
    {
        public GroupDto Result { get; set; }
    }
}
