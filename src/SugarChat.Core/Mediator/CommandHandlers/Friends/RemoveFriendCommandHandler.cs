using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Friends;
using SugarChat.Message.Commands.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Friends
{
    public class RemoveFriendCommandHandler : ICommandHandler<RemoveFriendCommand, SugarChatResponse>
    {
        private readonly IFriendService _friendService;

        public RemoveFriendCommandHandler(IFriendService friendService)
        {
            _friendService = friendService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<RemoveFriendCommand> context, CancellationToken cancellationToken)
        {
            var friendRemovedEvent = await _friendService.RemoveFriendAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(friendRemovedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
