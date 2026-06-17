using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.GroupUsers;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class QuitGroupCommandHandler : ICommandHandler<QuitGroupCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _groupUserService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public QuitGroupCommandHandler(IGroupUserService groupUserService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupUserService = groupUserService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }
        public async Task<SugarChatResponse> Handle(IReceiveContext<QuitGroupCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _groupUserService.QuitGroupAsync2(context.Message, cancellationToken));
            var groupQuittedEvent = await _groupUserService.QuitGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupQuittedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}