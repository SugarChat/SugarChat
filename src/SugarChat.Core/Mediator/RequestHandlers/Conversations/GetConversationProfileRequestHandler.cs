using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Shared.Dtos.Conversations;
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
            return new SugarChatResponse<ConversationDto>() { Code = 0, Message = "Success", Data = response.Result };
        }
    }
}
