using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Commands.Groups;

namespace SugarChat.Core.Mediator.CommandHandlers.Groups
{
    public class AddGroupCommandHandler : ICommandHandler<AddGroupCommand, AddGroupResponse>
    {
        private readonly IGroupService _groupService;

        public AddGroupCommandHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }


        public async Task<AddGroupResponse> Handle(IReceiveContext<AddGroupCommand> context, CancellationToken cancellationToken)
        {
            var groupAddedEvent =
                await _groupService.AddGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);

            await context.PublishAsync(groupAddedEvent, cancellationToken).ConfigureAwait(false);
            return new AddGroupResponse();
        }
    }
}