using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests.Groups
{
    public class GetGroupProfileRequest : IRequest
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }        
    }
}
