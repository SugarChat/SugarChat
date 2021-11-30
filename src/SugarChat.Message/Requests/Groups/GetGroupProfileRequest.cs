using Mediator.Net.Contracts;
using SugarChat.Message.Commands;

namespace SugarChat.Message.Requests.Groups
{
    public class GetGroupProfileRequest : IRequest, INeedUserExist
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}
