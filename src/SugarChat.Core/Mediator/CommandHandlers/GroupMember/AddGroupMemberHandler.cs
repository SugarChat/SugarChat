using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.GroupMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupMember
{
    public class AddGroupMemberHandler : ICommandHandler<AddGroupMemberCommand>
    {
        private readonly IGroupUserService _service;

        public async Task Handle(IReceiveContext<AddGroupMemberCommand> context, CancellationToken cancellationToken)
        {
            await _service.AddGroupMember(context.Message, cancellationToken);
        }
    }
}
