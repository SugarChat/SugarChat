using Mediator.Net.Contracts;
using SugarChat.Message.Commands;

namespace SugarChat.Message.Requests
{
    public class GetAllUnreadToUserRequest : IRequest, INeedUserExist
    {
        public string UserId { get; set; }
    }
}