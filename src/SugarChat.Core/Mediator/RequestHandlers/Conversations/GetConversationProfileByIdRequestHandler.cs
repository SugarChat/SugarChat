using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetConversationProfileByIdRequestHandler: IRequestHandler<GetConversationProfileByIdRequest, GetConversationProfileByIdResponse>
    {
        private readonly IConversationService _conversationService;
        public GetConversationProfileByIdRequestHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<GetConversationProfileByIdResponse> Handle(IReceiveContext<GetConversationProfileByIdRequest> context, CancellationToken cancellationToken)
        {
            return await _conversationService.GetConversationProfileByIdRequestAsync(context.Message, cancellationToken);
        }
    }
}
