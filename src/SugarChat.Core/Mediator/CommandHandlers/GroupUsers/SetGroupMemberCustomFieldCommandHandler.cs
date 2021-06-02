using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.GroupUsers;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class SetGroupMemberCustomFieldCommandHandler : ICommandHandler<SetGroupMemberCustomFieldCommand>
    {
        public IGroupUserService _groupUserService;
        public SetGroupMemberCustomFieldCommandHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }
        public async Task Handle(IReceiveContext<SetGroupMemberCustomFieldCommand> context, CancellationToken cancellationToken)
        {
            var groupMemberCustomFieldBeSetEvent = await _groupUserService.SetGroupMemberCustomFieldAsync(context.Message, cancellationToken);
            await context.PublishAsync(groupMemberCustomFieldBeSetEvent, cancellationToken);
        }
    }
}
