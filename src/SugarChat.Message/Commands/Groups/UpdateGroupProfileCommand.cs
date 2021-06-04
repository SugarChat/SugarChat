using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Groups
{
    public class UpdateGroupProfileCommand : ICommand
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public string Description { get; set; }
    }
}
