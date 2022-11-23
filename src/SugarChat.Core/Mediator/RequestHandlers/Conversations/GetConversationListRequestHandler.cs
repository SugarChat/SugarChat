using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Basic;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Paging;
using SugarChat.Message.Requests.Conversations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetConversationListRequestHandler : IRequestHandler<GetConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>
    {
        private readonly IConversationService _conversationService;
        public GetConversationListRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<SugarChatResponse<PagedResult<ConversationDto>>> Handle(IReceiveContext<GetConversationListRequest> context, CancellationToken cancellationToken)
        {
            var response = await _conversationService.GetConversationListByUserIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<PagedResult<ConversationDto>>() { Data = response };
        }
    }
}
