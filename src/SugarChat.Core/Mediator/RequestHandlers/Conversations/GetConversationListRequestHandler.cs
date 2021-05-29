using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetConversationListRequestHandler : IRequestHandler<GetConversationListRequest, GetConversationListResponse>
    {
        private readonly IConversationService _conversationService;
        public GetConversationListRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<GetConversationListResponse> Handle(IReceiveContext<GetConversationListRequest> context, CancellationToken cancellationToken)
        {
            return await _conversationService.GetConversationListByUserIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
        }
    }
}
