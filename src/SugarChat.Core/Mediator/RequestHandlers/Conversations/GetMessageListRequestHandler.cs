using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetMessageListRequestHandler : IRequestHandler<GetMessageListRequest, GetMessageListResponse>
    {
        private readonly IConversationService _conversationService;

        public GetMessageListRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }
        public async Task<GetMessageListResponse> Handle(IReceiveContext<GetMessageListRequest> context, CancellationToken cancellationToken)
        {
            return await _conversationService.GetPagingMessagesByConversationIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
        }
    }
}
