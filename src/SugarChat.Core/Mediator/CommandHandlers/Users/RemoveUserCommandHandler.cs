using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Users
{
    public class RemoveUserCommandHandler : ICommandHandler<RemoveUserCommand, SugarChatResponse>
    {
        public IUserService _userService;

        public RemoveUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<RemoveUserCommand> context, CancellationToken cancellationToken)
        {
            var userRemovedEvent = await _userService.RemoveUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(userRemovedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
