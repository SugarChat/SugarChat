using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Commands.Groups;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Groups
{

    public class UpdateGroupProfileCommandHandler : ICommandHandler<UpdateGroupProfileCommand, SugarChatResponse>
    {
        public IGroupService _groupService;
        public UpdateGroupProfileCommandHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<UpdateGroupProfileCommand> context, CancellationToken cancellationToken)
        {
            var groupProfileUpdatedEvent = await _groupService.UpdateGroupProfileAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupProfileUpdatedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
