using Mediator.Net.Context;
using Mediator.Net.Contracts;
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

        public BatchSetGroupMemberCustomFieldCommandHandler(IGroupUserService service)
        {
            _service = service;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchSetGroupMemberCustomFieldCommand> context, CancellationToken cancellationToken)
        {
            await _service.BatchSetGroupMemberCustomPropertiesAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
