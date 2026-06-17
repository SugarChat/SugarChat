using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Groups;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Groups
{

    public class UpdateGroupProfileCommandHandler : ICommandHandler<UpdateGroupProfileCommand, SugarChatResponse>
    {
        public IGroupService _groupService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public UpdateGroupProfileCommandHandler(IGroupService groupService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupService = groupService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<UpdateGroupProfileCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _groupService.UpdateGroupProfileAsync2(context.Message, cancellationToken));
            var groupProfileUpdatedEvent = await _groupService.UpdateGroupProfileAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupProfileUpdatedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
