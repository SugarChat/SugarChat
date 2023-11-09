using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Groups;

namespace SugarChat.Core.Mediator.CommandHandlers.Groups
{
    public class AddGroupCommandHandler : ICommandHandler<AddGroupCommand, SugarChatResponse>
    {
        private readonly IGroupService _groupService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public AddGroupCommandHandler(IGroupService groupService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupService = groupService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<AddGroupCommand> context, CancellationToken cancellationToken)
        {
            var groupAddedEvent =
                await _groupService.AddGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            _backgroundJobClientProvider.Enqueue(() => _groupService.AddGroupAsync2(context.Message, cancellationToken));
            await context.PublishAsync(groupAddedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}