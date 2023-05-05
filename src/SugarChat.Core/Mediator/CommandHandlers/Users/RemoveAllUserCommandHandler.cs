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
    public class RemoveAllUserCommandHandler : ICommandHandler<RemoveAllUserCommand, SugarChatResponse>
    {
        public IUserService _userService;

        public RemoveAllUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<RemoveAllUserCommand> context, CancellationToken cancellationToken)
        {
            await _userService.RemoveAllUserAsync(cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
