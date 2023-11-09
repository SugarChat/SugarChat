using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.GroupUsers;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUser
{
    public class AddGroupMemberCommandHandler : ICommandHandler<AddGroupMemberCommand, SugarChatResponse>    {

        private readonly IGroupUserService _service;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public AddGroupMemberCommandHandler(IGroupUserService service, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _service = service;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<AddGroupMemberCommand> context, CancellationToken cancellationToken)
        {
            var groupMemberAddedEvent = await _service.AddGroupMembersAsync(context.Message, cancellationToken).ConfigureAwait(false);
            _backgroundJobClientProvider.Enqueue(() => _service.AddGroupMembersAsync2(context.Message, cancellationToken));
            await context.PublishAsync(groupMemberAddedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
