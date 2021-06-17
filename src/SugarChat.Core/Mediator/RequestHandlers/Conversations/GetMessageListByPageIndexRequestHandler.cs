using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetMessageListByPageIndexRequestHandler : IRequestHandler<GetMessageListByPageIndexRequest, SugarChatResponse<IEnumerable<MessageDto>>>
    {
        private readonly IConversationService _conversationService;

        public GetMessageListByPageIndexRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> Handle(IReceiveContext<GetMessageListByPageIndexRequest> context, CancellationToken cancellationToken)
        {
            var response = await _conversationService.GetPagedMessagesByConversationIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<MessageDto>>() { Data = response };
        }
    }
}
