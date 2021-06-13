using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetMessageListRequestHandler : IRequestHandler<GetMessageListRequest, SugarChatResponse<MessageListResult>>
    {
        private readonly IConversationService _conversationService;

        public GetMessageListRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }
        public async Task<SugarChatResponse<MessageListResult>> Handle(IReceiveContext<GetMessageListRequest> context, CancellationToken cancellationToken)
        {
            var response = await _conversationService.GetPagedMessagesByConversationIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<MessageListResult>() { Data = response.Result };
        }
    }
}
