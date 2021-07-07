using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Requests.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetConversationByKeywordRequestHandler : IRequestHandler<GetConversationByKeywordRequest, SugarChatResponse<IEnumerable<ConversationDto>>>
    {
        private readonly IConversationService _conversationService;

        public GetConversationByKeywordRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<SugarChatResponse<IEnumerable<ConversationDto>>> Handle(IReceiveContext<GetConversationByKeywordRequest> context, CancellationToken cancellationToken)
        {
            var response = await _conversationService.GetConversationByKeyword(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<ConversationDto>>() { Data = response.Result };
        }
    }
}
