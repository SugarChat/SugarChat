using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Basic;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Requests.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetConversationProfileRequestHandler : IRequestHandler<GetConversationProfileRequest, SugarChatResponse<ConversationDto>>
    {
        private readonly IConversationService _conversationService;
        public GetConversationProfileRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<SugarChatResponse<ConversationDto>> Handle(IReceiveContext<GetConversationProfileRequest> context, CancellationToken cancellationToken)
        {
            var response = await _conversationService.GetConversationProfileByIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<ConversationDto>() { Data = response.Result };
        }
    }
}
