using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.GroupUsers
{
    public class RemoveUserFromGroupCommand : ICommand, INeedUserExist
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}