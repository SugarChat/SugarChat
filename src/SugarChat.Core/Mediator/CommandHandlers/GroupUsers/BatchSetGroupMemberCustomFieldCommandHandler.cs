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
    public class BatchSetGroupMemberCustomFieldCommandHandler : ICommandHandler<BatchSetGroupMemberCustomFieldCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _service;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public BatchSetGroupMemberCustomFieldCommandHandler(IGroupUserService service, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _service = service;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchSetGroupMemberCustomFieldCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _service.BatchSetGroupMemberCustomPropertiesAsync2(context.Message, cancellationToken));
            await _service.BatchSetGroupMemberCustomPropertiesAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
