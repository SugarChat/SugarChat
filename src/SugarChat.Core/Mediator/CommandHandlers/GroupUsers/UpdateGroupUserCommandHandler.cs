using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class UpdateGroupUserCommandHandler : ICommandHandler<UpdateGroupUserCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _groupUserService;

        public UpdateGroupUserCommandHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<UpdateGroupUserCommand> context, CancellationToken cancellationToken)
        {
            await _groupUserService.UpdateGroupUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}