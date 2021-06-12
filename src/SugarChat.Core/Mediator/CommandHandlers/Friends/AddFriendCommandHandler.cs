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
    public class AddFriendCommandHandler : ICommandHandler<AddFriendCommand, SugarChatResponse>
    {
        private readonly IFriendService _friendService;

        public AddFriendCommandHandler(IFriendService friendService)
        {
            _friendService = friendService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<AddFriendCommand> context, CancellationToken cancellationToken)
        {
            var friendAddedEvent = await _friendService.AddFriendAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(friendAddedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
