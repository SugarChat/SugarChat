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
    public class DismissGroupCommandHandler : ICommandHandler<DismissGroupCommand, SugarChatResponse>
    {
        private readonly IGroupService _groupService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public DismissGroupCommandHandler(IGroupService groupService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupService = groupService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }
        public async Task<SugarChatResponse> Handle(IReceiveContext<DismissGroupCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _groupService.DismissGroupAsync2(context.Message, cancellationToken));
            var groupDismissedEvent = await _groupService.DismissGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupDismissedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
