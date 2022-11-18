using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Basic;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Paging;
using SugarChat.Message.Requests.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetUnreadConversationListRequestHandler : IRequestHandler<GetUnreadConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>
    {
        private readonly IConversationService _conversationService;
        public GetUnreadConversationListRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }
        public async Task<SugarChatResponse<PagedResult<ConversationDto>>> Handle(IReceiveContext<GetUnreadConversationListRequest> context, CancellationToken cancellationToken)
        {
            var response = await _conversationService.GetUnreadConversationListByUserIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<PagedResult<ConversationDto>>() { Data = response };
        }
    }
}
