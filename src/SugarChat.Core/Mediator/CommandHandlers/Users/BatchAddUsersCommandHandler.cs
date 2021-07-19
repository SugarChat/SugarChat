using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Users
{
    public class BatchAddUsersCommandHandler : ICommandHandler<BatchAddUsersCommand, SugarChatResponse>
    {
        private readonly IUserService _userService;
        public BatchAddUsersCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchAddUsersCommand> context, CancellationToken cancellationToken)
        {
            var usersBatchAddedEvent = await _userService.BatchAddUsersAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(usersBatchAddedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
