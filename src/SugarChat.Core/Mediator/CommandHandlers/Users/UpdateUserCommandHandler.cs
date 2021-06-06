﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Users;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Users
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, SugarChatResponse>
    {
        public IUserService _userService;
        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<UpdateUserCommand> context, CancellationToken cancellationToken)
        {
            var userUpdatedEvent = await _userService.UpdateUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(userUpdatedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();

        }
    }
}
