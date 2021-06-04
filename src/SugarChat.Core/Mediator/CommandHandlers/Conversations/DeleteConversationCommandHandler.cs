﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Commands.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class DeleteConversationCommandHandler : ICommandHandler<DeleteConversationCommand,SugarChatResponse<object>>
    {
        public IConversationService _conversationService;
        public DeleteConversationCommandHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<SugarChatResponse<object>> Handle(IReceiveContext<DeleteConversationCommand> context, CancellationToken cancellationToken)
        {
            var conversationDeletedEvent = await _conversationService.DeleteConversationByConversationIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(conversationDeletedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<object>();
        }
    }
}