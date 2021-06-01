using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.GroupUsers;
using System;
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
        public Task Handle(IReceiveContext<SetGroupMemberCustomFieldCommand> context, CancellationToken cancellationToken)
        {
            return _groupUserService.SetGroupMemberCustomFieldAsync(context.Message, cancellationToken);
        }
    }
}
