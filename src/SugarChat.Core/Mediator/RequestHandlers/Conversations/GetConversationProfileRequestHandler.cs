using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetConversationProfileRequestHandler: IRequestHandler<GetConversationProfileRequest, GetConversationProfileResponse>
    {
        private readonly IConversationService _conversationService;
        public GetConversationProfileRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<GetConversationProfileResponse> Handle(IReceiveContext<GetConversationProfileRequest> context, CancellationToken cancellationToken)
        {
            return await _conversationService.GetConversationProfileByIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
        }
    }
}
