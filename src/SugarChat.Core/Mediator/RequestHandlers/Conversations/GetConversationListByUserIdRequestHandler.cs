﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetConversationListByUserIdRequestHandler : IRequestHandler<GetConversationListByUserIdRequest, GetConversationListByUserIdResponse>
    {
        private readonly IConversationService _conversationService;
        public GetConversationListByUserIdRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<GetConversationListByUserIdResponse> Handle(IReceiveContext<GetConversationListByUserIdRequest> context, CancellationToken cancellationToken)
        {
            return await _conversationService.GetConversationListByUserIdAsync(context.Message, cancellationToken);
        }
    }
}
